using Core.Service.Host;
using Core.Service.Host.ServiceDiscovery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Svc.Contract.Service;
using Svc.Implementation;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Core.Service.Host.ServiceDiscovery.Proxy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.ApiDescriptions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Test
{
    public class Startup : DiscoverableServiceStartup
    {
        protected override Type[] ServiceContractTypes => Array.Empty<Type>();

        public override void AddServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
        }

        public override void ServiceConfiguration(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

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
