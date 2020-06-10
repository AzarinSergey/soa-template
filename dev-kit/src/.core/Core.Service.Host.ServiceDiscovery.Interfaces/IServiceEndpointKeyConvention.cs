using System;

namespace Core.Service.Host.ServiceDiscovery.Interfaces
{
    public interface IServiceEndpointKeyConvention
    {
        string GetServiceEndpointKey(Type serviceInterfaceType);

        string GetServiceEndpointUri(string serviceName, Type serviceInterfaceType);

        string GetServiceKey(Type type);
    }
}