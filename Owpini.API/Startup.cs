﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using Owpini.Core.OwpiniEvents;
using Owpini.Core.OwpiniEvents.Dtos;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json.Serialization;

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

            services.AddScoped<IOwpiniRepository, OwpiniRepository>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<IUrlHelper>(implementationFactory =>
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>()
                .ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<ITypeHelperService, TypeHelperService>();

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory,
            OwpiniDbContext owpiniDbContext)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

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

            app.UseIdentity();

            owpiniDbContext.EnsureSeedDataForContext();

            app.UseMvc();
        }
    }
}
