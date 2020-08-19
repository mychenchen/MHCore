using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Crm.WebApp.AuthorizeHelper;
using Currency.Common.DI;
using Currency.Common.DIRegister;
using Currency.Common.Redis;
using Currency.Repository.DB_EF;
using Currency.Weixin;
using MH.WebApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Senparc.CO2NET;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.RegisterServices;
using System;
using System.IO;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using static Currency.Common.SystemTextJsonConvert;

namespace MH.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region app.config������

            services.Configure<WebAppSetting>(Configuration.GetSection("WebAppSetting")).AddMvc();
            services.Configure<SenparcWeixinSetting>(Configuration.GetSection("SenparcWeixinSetting")).AddMvc();

            #endregion

            #region ���ݿ��д

            var connection = Configuration.GetConnectionString("SqlServer");
            var connectionRead = Configuration.GetConnectionString("SqlReadServer");

            services.AddDbContext<MyDbContext>(options => options.UseSqlServer(connection, b => b.MigrationsAssembly("Currency.Repository")))
                .AddDbContext<MyReadDbContext>(options => options.UseSqlServer(connectionRead, b => b.MigrationsAssembly("Currency.Repository")))
                ;
            //���ݳ�ʼ��
            services.AddScoped<DefaultDataSeed>();


            #endregion

            #region ��� - ����ע��

            //services.AutoRegisterServicesFromAssembly("Currency.Service");
            services.AddAssembly("Currency.Service");
            #endregion

            #region ȫ�ֿ�������api����������

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(AuthorizeLogin));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            #endregion

            #region AutoMapperģ��ӳ��

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            #endregion

            #region Swagger�ӿڹ���

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hello", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, true); //��ӿ�������ע�ͣ�true��ʾ��ʾ������ע�ͣ�
            });
            #endregion

            #region ȫ��DI,��ȡָ������

            var builder = new ContainerBuilder();
            builder.Populate(services);
            DI.Current = builder.Build();

            new AutofacServiceProvider(DI.Current);
            #endregion

            #region ����

            services.AddCors(options =>
            {
                options.AddPolicy("allow_all", q =>
                {
                    //buildler.WithOrigins("http://localhost:49554")
                    q
                    .SetIsOriginAllowed(origin => true)
                    //.AllowAnyOrigin() //�����κ���Դ����������  SignalR 2.2������ʹ�� 
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();//ָ������cookie
                });
            });

            #endregion

            #region Senparc΢��

            services.AddSenparcGlobalServices(Configuration)//Senparc.CO2NET ȫ��ע��
                    .AddSenparcWeixinServices(Configuration);//Senparc.Weixin ע��

            #endregion

            #region ���ݷ���,�Զ���

            services.AddControllers().AddJsonOptions(options =>
            {
                var encoderSettings = new TextEncoderSettings();
                encoderSettings.AllowRanges(UnicodeRanges.All);
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(encoderSettings);//ȫ��תΪutf-8
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;//��ʹ���շ�������,����ԭ����
                options.JsonSerializerOptions.Converters.Add(new DateTimeTxtConverter()); //ʱ��ȥ�� T
                options.JsonSerializerOptions.Converters.Add(new DateTimeTxtNullableConverter()); //ʱ��ȥ�� T
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            #endregion

            //����mq��Ϣ���ճ���
            //DI.GetService<IMqReceive>().ReceiveAll();

            #region ע���Զ��幹�캯��

            //ע��΢�Ű�����
            services.AddScoped<BasicApi>();

            //ע��redis������
            services.AddScoped<RedisManager>();

            #endregion

        }

        /// <summary>
        ///  This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="senparcSetting"></param>
        /// <param name="senparcWeixinSetting"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<SenparcSetting> senparcSetting, IOptions<SenparcWeixinSetting> senparcWeixinSetting)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //������,��ֱ�ӷ��ʾ�̬ҳ��,��̬�ļ�
            app.UseStaticFiles();

            //�Զ�����Դ
            app.UseStaticFiles(new StaticFileOptions
            {
                //��Դ���ڵľ���·����
                FileProvider = new PhysicalFileProvider(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
                //��ʾ����·��,����'/'��ͷ
                RequestPath = "/uploads"
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            #region ���� Ĭ������ ��ӳ���

            var myContext = DI.GetService<MyDbContext>();
            DefaultDataSeed.SeedAsync(myContext).Wait();

            #endregion

            #region Senparc΢��

            // ���� CO2NET ȫ��ע�ᣬ���룡
            IRegisterService register = RegisterService.Start(senparcSetting.Value)
                                                        .UseSenparcGlobal(false, null);

            //��ʼע��΢����Ϣ�����룡
            register.UseSenparcWeixin(senparcWeixinSetting.Value, senparcSetting.Value);

            #endregion

            app.UseCors("allow_all");

            //�Զ�������ʼ����ַ
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //c.RoutePrefix = "swagger";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHelp V1");
            });

        }
    }
}
