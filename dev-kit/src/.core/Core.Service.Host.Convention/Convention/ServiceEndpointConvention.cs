using System;
using System.Reflection;
using Core.Service.Host.Convention.Configuration;

namespace Core.Service.Host.Convention.Convention
{
    public class ServiceEndpointConvention : IServiceEndpointConvention
    {
        public string GetServiceHost(ApplicationConfig appConfig, string serviceName)
            => $"{appConfig.ApplicationName}.{serviceName}";

        public string GetServiceMethodPath(MethodInfo methodInfo, Type serviceInterfaceType)
        {
            var serviceInterfaceName = serviceInterfaceType.Name;
            return $"{(serviceInterfaceName.StartsWith("I") ? serviceInterfaceName.Substring(1) : serviceInterfaceName)}/{methodInfo.Name}";
        }
    }
}