using Cmn.Constants;
using Core.Service.Host;
using Core.Service.Host.Client.ServiceCollectionExtensions;
using Exs.Contract.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using Bsp.Contract.Service;

namespace Api.Test
{
    public class Startup : StatelessServiceStartup
    {
        protected override Type[] ServiceContractTypes => Array.Empty<Type>();

        public override void RegisterStatelessService(IServiceCollection services)
        {
            services.RegisterInternalServiceProxy<IExampleServiceState>(ServiceNames.BackendExample);
            services.RegisterInternalServiceProxy<IBookShopService>(ServiceNames.BookShop);
        }

        public override void ServiceConfiguration(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}