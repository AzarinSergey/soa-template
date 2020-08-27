using System;

namespace Core.Service.Host.ServiceDiscovery.Interfaces
{
    public interface IServiceEndpointConvention
    {
        string GetServiceEndpointKey(Type serviceInterfaceType);

        string GetServiceEndpointUri(string serviceName, Type serviceInterfaceType);

        string GetServiceKey(Type type);
    }
}