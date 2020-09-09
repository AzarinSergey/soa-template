using Core.Service.Host;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Svc.Contract.Service;
using Svc.Implementation.Service;
using System;

namespace Svc.Implementation
{
    public class Startup : StatelessServiceStartup
    {
        protected override Type[] ServiceContractTypes => new []
            {
                typeof(IExampleServiceState)
            };

        public override void RegisterStatelessService(IServiceCollection c)
        {
            c.AddSingleton<IExampleServiceState, ExampleService>();
            c.AddSingleton<ExampleService>();
            c.AddHostedService(provider => provider.GetService<ExampleService>());
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
