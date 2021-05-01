using Bsp.Contract.Service;
using Bsp.Implementation.Service;
using Core.Service.Host;
using Core.Service.Host.ServiceCollectionExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bsp.Implementation
{
    public class Startup : StatelessServiceStartup
    {
        protected override Type[] ServiceContractTypes => new[]
        {
            typeof(IBookShopService)
        };

        public override void RegisterStatelessService(IServiceCollection c)
        {
            c.RegisterStatelessServices()
                .AddHttpService<BookShopService, IBookShopService>()
                .AddBackgroundService<BookShopService>();
        }

        public override void ServiceConfiguration(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Alive");
                });
            });
        }
    }
}
