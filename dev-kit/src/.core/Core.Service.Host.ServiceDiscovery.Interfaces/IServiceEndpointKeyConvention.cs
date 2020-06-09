using System;

namespace Core.Service.Host.ServiceDiscovery.Interfaces
{
    public interface IServiceEndpointKeyConvention
    {
        string GetServiceKey(Type serviceInterfaceType);
    }
}