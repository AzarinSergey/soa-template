using Core.Service.Host;
using Core.Service.Host.ServiceCollectionExtensions;
using Exs.Contract.Service;
using Exs.Implementation.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Exs.Implementation
{
    public class Startup : StatelessServiceStartup
    {
        protected override Type[] ServiceContractTypes => new []
            {
                typeof(IExampleServiceState)
            };

        public override void RegisterStatelessService(IServiceCollection c)
        {
            c.RegisterStatelessServices()
                .AddHttpService<ExampleService, IExampleServiceState>()
                .AddBackgroundService<ExampleService>();
        }

        public override void ServiceConfiguration(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseEndpoints(builder =>
            {
                builder.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Alive");
                });
            });
        }
    }
}
