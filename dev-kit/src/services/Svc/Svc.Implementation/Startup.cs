using Core.Service.ServiceDiscovery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Svc.Implementation
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile($"appsettings.json", reloadOnChange: true, optional: false)
                .AddJsonFile($"appsettings.{environmentName}.json", reloadOnChange: true, optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConsul(Configuration);
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

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });

            app.UseConsul(healthPath);
        }
    }
}
