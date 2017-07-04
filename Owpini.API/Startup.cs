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
using System.Linq;
using Microsoft.AspNetCore.Diagnostics;

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
            services.AddMvc(setupAction => {
                setupAction.ReturnHttpNotAcceptable = true;
                setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());

                var xmlDataContractSerializerInputFormatter =
                new XmlDataContractSerializerInputFormatter();

                xmlDataContractSerializerInputFormatter.SupportedMediaTypes
                .Add("application/vnd.owpini.full+xml");

                setupAction.InputFormatters.Add(xmlDataContractSerializerInputFormatter);

                var jsonInputFormatter = setupAction.InputFormatters
                .OfType<JsonInputFormatter>().FirstOrDefault();

                if (jsonInputFormatter != null)
                {
                    jsonInputFormatter.SupportedMediaTypes
                    .Add("application/vnd.owpini.business.full+json");
                }

                var jsonOutputFormatter = setupAction.OutputFormatters
                    .OfType<JsonOutputFormatter>().FirstOrDefault();

                if (jsonOutputFormatter != null)
                {
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.owpini.hateoas+json");
                }

            })
            .AddJsonOptions(options => {
                options.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            });

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

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (exceptionHandlerFeature != null)
                        {
                            var logger = loggerFactory.CreateLogger("Global exception logger");
                            logger.LogError(500,
                                exceptionHandlerFeature.Error,
                                exceptionHandlerFeature.Error.Message);
                        }

                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");

                    });
                });
            }

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

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/OwpiniApi/swagger.json", "Owpini API");
            });

            app.UseMvc();
        }
    }
}
