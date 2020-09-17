using System;
using System.Reflection;
using Core.Service.Host.Convention.Configuration;

namespace Core.Service.Host.Convention.Convention
{
    public interface IServiceEndpointConvention
    {
        string GetServiceHost(ApplicationConfig appConfig, string serviceName);
        string GetServiceMethodPath(MethodInfo method, Type serviceInterfaceType);
    }
}