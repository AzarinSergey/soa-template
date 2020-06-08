using Core.Service.Host;
using Core.Service.Host.ServiceDiscovery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Svc.Contract.Service;
using Svc.Implementation;
using System;

namespace Api.Test
{
    public class Startup : DiscoverableServiceStartup
    {
        protected override Type[] ServiceContractTypes => Array.Empty<Type>();

        public override void AddServices(IServiceCollection c)
        {
            
        }

        public override void ServiceConfiguration(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //TODO: добавить MVC роутинг
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    var proxy = ServiceProxy.Create<IExampleServiceState>();
                    var result = await proxy.GetState(1, new CrossContext { Uuid = Guid.NewGuid().ToString() }, context.RequestAborted);

                    await context.Response.WriteAsync($"Response value: {result}");
                });
            });
        }
    }
}
