using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Owpini.EntityFramework;
using Owpini.EntityFramework.EntityFramework.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Owpini.EntityFramework.SeedData;
using Owpini.Core.Businesses.Dtos;
using Owpini.Core.Businesses;
using Owpini.Core.OwpiniUsers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NLog.Extensions.Logging;
using Owpini.Core.OwpiniEvents;
using Owpini.Core.OwpiniEvents.Dtos;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json.Serialization;
using AspNetCoreRateLimit;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Http;
using NLog.Web;

namespace Owpini.API
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<OwpiniDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<OwpiniUser, IdentityRole>()
                .AddEntityFrameworkStores<OwpiniDbContext>()
                .AddDefaultTokenProviders();

            services.AddResponseCaching();

            services.AddSwaggerGen(setupAction =>
            {
                setupAction.SwaggerDoc("OwpiniApi",
                    new Info { Title = "Owpini API Documentation" });
            });

            // Add framework services.
            services.AddMvc(setupAction => {
                setupAction.ReturnHttpNotAcceptable = true;
                setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                setupAction.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());
            })
            .AddJsonOptions(options => {
                options.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            });

            services.AddMemoryCache();

            services.Configure<IpRateLimitOptions>((options) =>
            {
                options.GeneralRules = new List<RateLimitRule>()
                {
                    new RateLimitRule()
                    {
                        Endpoint = "*",
                        Limit = 1000,
                        Period = "5m"
                    },
                    new RateLimitRule()
                    {
                        Endpoint = "*",
                        Limit = 200,
                        Period = "10s"
                    }
                };
            });

            services.AddScoped<IOwpiniRepository, OwpiniRepository>();
            services.AddScoped<IUrlHelper>(implementationFactory =>
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>()
                .ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<ITypeHelperService, TypeHelperService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory,
            OwpiniDbContext owpiniDbContext)
        {
            app.UseIdentity();

            loggerFactory.AddNLog();

            app.AddNLogWeb();

            env.ConfigureNLog("nlog.config");

            app.UseResponseCaching();

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Business, BusinessDto>();
                cfg.CreateMap<Business, BusinessForUpdateDto>();
                cfg.CreateMap<BusinessDto, Business>();
                cfg.CreateMap<BusinessForCreationDto, Business>();
                cfg.CreateMap<BusinessForUpdateDto, Business>();

                cfg.CreateMap<OwpiniEvent, OwpiniEventDto>();
                cfg.CreateMap<OwpiniEventDto, OwpiniEvent>();

            });

            owpiniDbContext.EnsureSeedDataForContext();

            app.UseIpRateLimiting();

            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/OwpiniApi/swagger.json", "Owpini API");
            });
        }
    }
}
