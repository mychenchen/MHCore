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
            #region app.config配置项

            services.Configure<WebAppSetting>(Configuration.GetSection("WebAppSetting")).AddMvc();
            services.Configure<SenparcWeixinSetting>(Configuration.GetSection("SenparcWeixinSetting")).AddMvc();

            #endregion

            #region 数据库读写

            var connection = Configuration.GetConnectionString("SqlServer");
            var connectionRead = Configuration.GetConnectionString("SqlReadServer");

            services.AddDbContext<MyDbContext>(options => options.UseSqlServer(connection, b => b.MigrationsAssembly("Currency.Repository")))
                .AddDbContext<MyReadDbContext>(options => options.UseSqlServer(connectionRead, b => b.MigrationsAssembly("Currency.Repository")))
                ;
            //数据初始化
            services.AddScoped<DefaultDataSeed>();


            #endregion

            #region 类库 - 批量注入

            //services.AutoRegisterServicesFromAssembly("Currency.Service");
            services.AddAssembly("Currency.Service");
            #endregion

            #region 全局控制器与api控制器拦截

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(AuthorizeLogin));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            #endregion

            #region AutoMapper模型映射

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            #endregion

            #region Swagger接口管理

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hello", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, true); //添加控制器层注释（true表示显示控制器注释）
            });
            #endregion

            #region 全局DI,获取指定内容

            var builder = new ContainerBuilder();
            builder.Populate(services);
            DI.Current = builder.Build();

            new AutofacServiceProvider(DI.Current);
            #endregion

            #region 跨域

            services.AddCors(options =>
            {
                options.AddPolicy("allow_all", q =>
                {
                    //buildler.WithOrigins("http://localhost:49554")
                    q
                    .SetIsOriginAllowed(origin => true)
                    //.AllowAnyOrigin() //允许任何来源的主机访问  SignalR 2.2不允许使用 
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();//指定处理cookie
                });
            });

            #endregion

            #region Senparc微信

            services.AddSenparcGlobalServices(Configuration)//Senparc.CO2NET 全局注册
                    .AddSenparcWeixinServices(Configuration);//Senparc.Weixin 注册

            #endregion

            #region 数据返回,自定义

            services.AddControllers().AddJsonOptions(options =>
            {
                var encoderSettings = new TextEncoderSettings();
                encoderSettings.AllowRanges(UnicodeRanges.All);
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(encoderSettings);//全部转为utf-8
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;//不使用驼峰命名发,返回原数据
                options.JsonSerializerOptions.Converters.Add(new DateTimeTxtConverter()); //时间去除 T
                options.JsonSerializerOptions.Converters.Add(new DateTimeTxtNullableConverter()); //时间去除 T
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            #endregion

            //启动mq消息接收程序
            //DI.GetService<IMqReceive>().ReceiveAll();

            #region 注入自定义构造函数

            //注入微信帮助类
            services.AddScoped<BasicApi>();

            //注入redis帮助类
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

            //开启后,可直接访问静态页面,静态文件
            app.UseStaticFiles();

            //自定义资源
            app.UseStaticFiles(new StaticFileOptions
            {
                //资源所在的绝对路径。
                FileProvider = new PhysicalFileProvider(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
                //表示访问路径,必须'/'开头
                RequestPath = "/uploads"
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            #region 启动 默认数据 添加程序

            var myContext = DI.GetService<MyDbContext>();
            DefaultDataSeed.SeedAsync(myContext).Wait();

            #endregion

            #region Senparc微信

            // 启动 CO2NET 全局注册，必须！
            IRegisterService register = RegisterService.Start(senparcSetting.Value)
                                                        .UseSenparcGlobal(false, null);

            //开始注册微信信息，必须！
            register.UseSenparcWeixin(senparcWeixinSetting.Value, senparcSetting.Value);

            #endregion

            app.UseCors("allow_all");

            //自定义程序初始化地址
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
