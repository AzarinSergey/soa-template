using System;
using System.Reflection;

namespace Core.Service.Host.ServiceDiscovery.Interfaces
{
    public interface IApplicationConfig
    {
        string ApplicationName { get; set; }
    }

    public interface IServiceConfig
    {
        string ServiceName { get; set; }
    }

    public interface IServiceEndpointConvention
    {
        string GetServiceHost(IApplicationConfig appCfg, string serviceName);
        string GetServiceMethodPath(MethodInfo method, Type serviceInterfaceType);
    }
}