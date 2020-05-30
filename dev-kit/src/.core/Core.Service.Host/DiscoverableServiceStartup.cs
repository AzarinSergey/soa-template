using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Core.Service.Host.ServiceDiscovery;

namespace Core.Service.Host
{
    public abstract class DiscoverableServiceStartup
    {
        protected DiscoverableServiceStartup()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile($"appsettings.json", reloadOnChange: true, optional: false)
                .AddJsonFile($"appsettings.{environmentName}.json", reloadOnChange: true, optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        protected IConfigurationRoot Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConsul(Configuration);
            AddServices(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            const string healthPath = "/tool/health";

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet(healthPath, async context =>
                {
                    await context.Response.WriteAsync("===> OK <===");
                });
            });

            app.UseConsul(healthPath);

            ServiceConfiguration(app, env);
        }


        public abstract void AddServices(IServiceCollection c);
        public abstract void ServiceConfiguration(IApplicationBuilder app, IWebHostEnvironment env);
    }
}
