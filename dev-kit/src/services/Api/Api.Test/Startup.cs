using Consul;
using Core.Service.Host.ServiceDiscovery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Svc.Contract.Service;
using Svc.Implementation;
using System;

namespace Api.Test
{
    public class Startup
    {
        //TODO: вынести задубилрованныу логику в базывый стартап
        public Startup()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile($"appsettings.json", reloadOnChange: true, optional: false)
                .AddJsonFile($"appsettings.{environmentName}.json", reloadOnChange: true, optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public IConfigurationRoot Configuration { get; set; }

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

            HttpServiceProxy.ReverseProxyAddress = Configuration.Get<ServiceConfig>().ReverseProxyAddress;
            HttpServiceProxy.Client = app.ApplicationServices.GetRequiredService<IConsulClient>();

            app.UseRouting();

            //TODO: добавить MVC роутинг
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    var result = await HttpServiceProxy
                        .Call<IExampleServiceState, string>(svc => svc.GetState(1, new CrossContext(), context.RequestAborted));

                    await context.Response.WriteAsync($"Response value: {result}");
                });
            });
        }
    }
}
