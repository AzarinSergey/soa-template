using Core.Service.Host.ServiceDiscovery.Interfaces;
using System;
using System.Reflection;

namespace Core.Service.Host.ServiceDiscovery
{
    public class ApplicationConfig : IApplicationConfig
    {
        public const string SectionName = "ApplicationConfig";

        public string ApplicationName { get; set; }
    }

    public class ServiceConfig : IServiceConfig
    {
        public const string SectionName = "ServiceConfig";

        public string ServiceName { get; set; }
    }

    public class ServiceEndpointConvention : IServiceEndpointConvention
    {
        public string GetServiceHost(IApplicationConfig appCfg, string serviceName)
            => $"{appCfg.ApplicationName}.{serviceName}";

        public string GetServiceMethodPath(MethodInfo methodInfo, Type serviceInterfaceType)
        {
            var serviceInterfaceName = serviceInterfaceType.Name;
            return $"{(serviceInterfaceName.StartsWith("I") ? serviceInterfaceName.Substring(1) : serviceInterfaceName)}/{methodInfo.Name}";
        }
    }
}