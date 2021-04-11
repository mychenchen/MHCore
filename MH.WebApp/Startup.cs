using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using Crm.WebApp.AuthorizeHelper;
using Currency.Aop;
using Currency.Common.Caching;
using Currency.Common.DI;
using Currency.Common.DIRegister;
using Currency.Common.LogManange;
using Currency.Common.Redis;
using Currency.Models.Comm_Entity;
using Currency.Repository.DB_EF;
using Currency.Weixin;
using IdentityModel;
using MH.WebApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Senparc.CO2NET;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.RegisterServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using static Currency.Common.SystemTextJsonConvert;

namespace MH.WebApp
{
    public class Startup
    {
        public static SymmetricSecurityKey symmetricKey;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region app.config配置项

            services.Configure<SenparcWeixinSetting>(Configuration.GetSection("SenparcWeixinSetting"));
            services.Configure<WebAppSetting>(Configuration.GetSection("WebAppSetting"));
            services.Configure<SugarConfigSetting>(Configuration.GetSection("SugarSettings"));

            #endregion

            #region 注入自定义构造函数

            //注入微信帮助类-单例
            services.AddSingleton<BasicApi>();

            //注入redis帮助类-单例
            services.AddSingleton<RedisManager>();

            //注入cache缓存类-单例
            services.AddSingleton<CoreMemoryCache>();

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

            #region AutoMapper模型映射

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            #endregion

            //#region AOP切面-注入

            ////注册用户维护业务层
            //var basePath = AppContext.BaseDirectory;
            //var serviceDll = Path.Combine(basePath, "Currency.Aop");

            //if (File.Exists(serviceDll))
            //{
            //    ////注册AOP拦截器
            //    //services.RegisterType(typeof(OnLogExecuting));
            //    services.RegisterAssemblyTypes(Assembly.LoadFrom(serviceDll))
            //        .AsImplementedInterfaces()
            //        .EnableInterfaceInterceptors()//开启切面，需要引入Autofac.Extras.DynamicProxy
            //        .InterceptedBy(typeof(UserAop));//指定拦截器，可以指定多个

            //}
            //#endregion

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
                options.AddPolicy("any", q =>
                {
                    q
                      //q.WithOrigins("http://localhost:49554")
                      //.SetIsOriginAllowed(origin => true)
                      .AllowAnyOrigin() //允许任何来源的主机访问  SignalR 2.2不允许使用 
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

            #region JWT验证

            var webSetting = Configuration.GetSection("WebAppSetting");
            //生成密钥
            var keyByteArray = Encoding.ASCII.GetBytes(webSetting.GetValue<string>("JwtSecret"));
            symmetricKey = new SymmetricSecurityKey(keyByteArray);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = JwtClaimTypes.Role,

                    ValidIssuer = webSetting.GetValue<string>("JwtIss"), //发行人
                    ValidAudience = webSetting.GetValue<string>("JwtAud"),  //订阅人
                    IssuerSigningKey = symmetricKey


                    /***********************************TokenValidationParameters的参数默认值***********************************/
                    // RequireSignedTokens = true,
                    // SaveSigninToken = false,
                    // ValidateActor = false,
                    // 将下面两个参数设置为false，可以不验证Issuer和Audience，但是不建议这样做。
                    // ValidateAudience = true,
                    // ValidateIssuer = true, 
                    // ValidateIssuerSigningKey = false,
                    // 是否要求Token的Claims中必须包含Expires
                    // RequireExpirationTime = true,
                    // 允许的服务器时间偏移量
                    // ClockSkew = TimeSpan.FromSeconds(300),
                    // 是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                    // ValidateLifetime = true
                };
            });
            #endregion

            #region 全局控制器与api控制器拦截

            services.AddMvc(options =>
            {
                //options.Filters.Add(typeof(AuthorizeLogin)); 开启后,可使用 jwt 替换token 
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

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

            var pathFile = Directory.GetCurrentDirectory() + @"/uploads";
            if (!Directory.Exists(pathFile))
            {
                Directory.CreateDirectory(pathFile);
            }

            //自定义资源
            app.UseStaticFiles(new StaticFileOptions
            {
                //资源所在的绝对路径。
                FileProvider = new PhysicalFileProvider(pathFile),
                //表示访问路径,必须'/'开头
                RequestPath = "/uploads"
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
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

            LogHelper.Configure(); //使用前先配置

            //app.UseCors("any");

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
