using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Core.Service.Host;

namespace Svc.Implementation
{
    public class Startup : DiscoverableServiceStartup
    {
        public override void AddServices(IServiceCollection c)
        { }

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
