using System.Collections.Generic;
using System.Reflection;

namespace Core.Service.Host.Swagger
{
    public class ServiceRoute
    {
        public ServiceRoute(string serviceInterfaceUri, string routeTemplate)
        {
            ServiceInterfaceUri = serviceInterfaceUri;
            RouteTemplate = routeTemplate;
        }

        public string ServiceInterfaceUri { get; }

        public string RouteTemplate { get; }
    }
}