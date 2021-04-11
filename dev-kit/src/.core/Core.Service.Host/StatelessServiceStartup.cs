using Core.Service.Host.ApplicationBuilderExtensions;
using Core.Service.Host.Convention.Configuration;
using Core.Service.Host.Convention.Convention;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapGet(healthPath, async context =>
                {
                    await context.Response.WriteAsync($"===> OK <=== \n {Tool.Tools.Json.Serializer.Serialize(context.Request.Headers)}");
                });
            });

            var serviceEndpointConvention = app.ApplicationServices.GetRequiredService<IServiceEndpointConvention>();

            app.UseServiceEndpoints(ServiceContractTypes, serviceEndpointConvention);

            ServiceConfiguration(app, env);
        }


        public abstract void RegisterStatelessService(IServiceCollection c);
        public abstract void ServiceConfiguration(IApplicationBuilder app, IWebHostEnvironment env);
    }
}
