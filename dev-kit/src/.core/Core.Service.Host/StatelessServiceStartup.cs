﻿using Core.Service.Host.ApplicationBuilderExtensions;
using Core.Service.Host.Convention.Configuration;
using Core.Service.Host.Convention.Convention;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Tool;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Core.Service.Host
{
    public abstract class StatelessServiceStartup
    {
        protected abstract Type[] ServiceContractTypes { get; }

        protected StatelessServiceStartup()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile($"appsettings.json", reloadOnChange: true, optional: false)
                .AddJsonFile($"appsettings.{environmentName}.json", reloadOnChange: true, optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        protected IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<IServiceEndpointConvention, ServiceEndpointConvention>();

            services.Configure<ServiceConfig>(Configuration.GetSection(ServiceConfig.SectionName));
            services.Configure<ApplicationConfig>(Configuration.GetSection(ApplicationConfig.SectionName));

            services.AddControllers();
            RegisterStatelessService(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            const string healthPath = "/tool/health";
            const string printEnvPath = "/tool/printEnv";
            const string printOptPath = "/tool/printOpt";

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapGet(healthPath, async context =>
                {
                    await context.Response.WriteAsync($"===> OK <=== \n {Tool.Tools.Json.Serializer.Serialize(context.Request.Headers)}");
                });

                endpoints.MapGet(printEnvPath, async context =>
                {
                    await context.Response.WriteAsync($"{Tools.Json.Serializer.Serialize(Environment.GetEnvironmentVariables(), Formatting.Indented)}");
                });

                endpoints.MapGet(printOptPath, async context =>
                {
                    await context.Response.WriteAsync($"{Tools.Json.Serializer.Serialize(ResolveServiceConfigObject(app), Formatting.Indented)}");
                });
            });

            var serviceEndpointConvention = app.ApplicationServices.GetRequiredService<IServiceEndpointConvention>();

            app.UseServiceEndpoints(ServiceContractTypes, serviceEndpointConvention);

            ServiceConfiguration(app, env);
        }


        public abstract void RegisterStatelessService(IServiceCollection c);
        public abstract void ServiceConfiguration(IApplicationBuilder app, IWebHostEnvironment env);
        public virtual ServiceConfig ResolveServiceConfigObject(IApplicationBuilder app)
            => app.ApplicationServices.GetRequiredService<IOptions<ServiceConfig>>().Value;
    }
}
