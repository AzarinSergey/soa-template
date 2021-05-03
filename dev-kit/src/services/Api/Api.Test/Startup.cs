using Cmn.Constants;
using Core.Service.Host;
using Core.Service.Host.Client.ServiceCollectionExtensions;
using Exs.Contract.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using Bsp.Contract.Service;
using Core.Service.Host.Convention.Configuration;
using Microsoft.Extensions.Options;

namespace Api.Test
{
    public class Startup : StatelessServiceStartup
    {
        protected override Type[] ServiceContractTypes => Array.Empty<Type>();

        public override void RegisterStatelessService(IServiceCollection services)
        {
            services.RegisterInternalServiceProxy<IExampleServiceState>(ServiceNames.BackendExample);
            services.RegisterInternalServiceProxy<IBookShopService>(ServiceNames.BookShop);

            services.Configure<ApiTestServiceConfig>(Configuration.GetSection(ServiceConfig.SectionName));
        }

        public override void ServiceConfiguration(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
        }

        public override ServiceConfig ResolveServiceConfigObject(IApplicationBuilder app)
            => app.ApplicationServices.GetRequiredService<IOptions<ApiTestServiceConfig>>().Value;
        
    }
}