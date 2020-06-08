using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Collections.Generic;

namespace Api.Test
{
    //SwaggerGenerator
    //SchemaGenerator
    //Для генерации сваггера по интерфейсу надо определить IApiDescriptionGroupCollectionProvider из MVC модуля
    //Этот интерфейс по OpenApi сделан и его сваггер использует для генерации json
    public class Test : IApiDescriptionGroupCollectionProvider
    {
        public Test()
        {
            var apiDescriptions = new List<ApiDescription>
            {
                new ApiDescription//вот эта сущность используется генератором сваггера
                {
                    ActionDescriptor = new InterfaceMethodDescriptor(),
                    GroupName = "ServiceInterfaceName",
                    HttpMethod = null,//судя по коду null не поддерживается сваггером - каждый метод в интерфейсе надо помечать аттрибутом
                    RelativePath = "path to interface endpoint",//этот путь используется сваггером как путь к контроллеру? или сразу до метода?
                    //ParameterDescriptions = { }
                    //Properties = { }
                    //SupportedRequestFormats = { }
                    //SupportedResponseTypes = { }
                }
            };

            var apiDescGroups = new List<ApiDescriptionGroup>
            {
                new ApiDescriptionGroup("ServiceInterfaceName", apiDescriptions)
            };

            ApiDescriptionGroups = new ApiDescriptionGroupCollection(apiDescGroups, 0);
        }
        public ApiDescriptionGroupCollection ApiDescriptionGroups { get; }
    }

    public class InterfaceMethodDescriptor : ActionDescriptor
    {
        //непонятно пока что из этого юзается сваггером
        public InterfaceMethodDescriptor()
        {

        }
    }
}
