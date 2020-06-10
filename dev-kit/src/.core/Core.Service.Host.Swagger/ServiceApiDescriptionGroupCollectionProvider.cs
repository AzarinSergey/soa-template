using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core.Service.Host.Swagger
{
    public class ServiceApiDescriptionGroupCollectionProvider : IApiDescriptionGroupCollectionProvider
    {
        public ServiceApiDescriptionGroupCollectionProvider()
        { }

        public ApiDescriptionGroupCollection ApiDescriptionGroups => new ApiDescriptionGroupCollection(GetGroups(), 0);

        private IReadOnlyList<ApiDescriptionGroup> GetGroups()
        {
            return new Collection<ApiDescriptionGroup>
            {
                new ApiDescriptionGroup("InterfaceName", new []
                {
                    new ApiDescription
                    {
                        
                    }
                })
            };
        }
    }
}
