using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Core.Service.Host.ServiceDiscovery;
using Core.Service.Host.ServiceDiscovery.Interfaces;
using Core.Tool;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Service.Host.ApplicationBuilderExtensions
{
    public static class ServiceEndpointUsing
    {

        public static IApplicationBuilder UseServiceEndpoints(this IApplicationBuilder app, Type[] serviceTypes,
            ServiceDiscoveryConfig settings, IServiceEndpointConvention serviceEndpointConvention)
        {
            foreach (var serviceType in serviceTypes)
            {
                var instance = app.ApplicationServices.GetService(serviceType);
                var servicePath = serviceEndpointConvention.GetServiceEndpointUri(settings.ServiceName, serviceType);
                var methods = serviceType.GetMethods();

                app.UseEndpoints(builder =>
                {
                    foreach (var methodInfo in methods)
                    {
                        builder.MapPost($"{servicePath}/{methodInfo.Name}", context => MapPost(context, methodInfo, instance));
                    }
                });
            }
            return app;
        }

        public static IApplicationBuilder UseServiceEndpoints(this IApplicationBuilder app, Type[] serviceTypes,
            ServiceDiscoveryConfig settings)
            =>    UseServiceEndpoints(app, serviceTypes, settings,
                app.ApplicationServices.GetRequiredService<IServiceEndpointConvention>());

        private static async Task MapPost(HttpContext context, MethodInfo methodInfo, object instance)
        {
            var cancellationToken = context.RequestAborted;

            string jsonBody;

            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                jsonBody = await reader.ReadToEndAsync();

            var methodParams = methodInfo.GetParameters()
                .Where(x => x.ParameterType.FullName != typeof(CancellationToken).FullName)
                .ToArray();
            var requestParams = new object[methodParams.Length + 1];
            var requestParamsJson = JsonDocument.Parse(jsonBody);

            for (var i = 0; i < methodParams.Length; i++)
            {
                if (requestParamsJson.RootElement.TryGetProperty(methodParams[i].Name, out var element))
                {
                    requestParams[i] = Tools.Json.Serializer.Deserialize(element.GetRawText(), methodParams[i].ParameterType);
                }
                else
                {
                    throw new KeyNotFoundException($"Request parameter not found by name: '{methodParams[i].Name}' ");
                }
            }

            requestParams[^1] = cancellationToken;

            var task = (Task)methodInfo.Invoke(instance, requestParams);
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result");
            var resultValue = resultProperty?.GetValue(task);

            await context.Response.WriteAsync(Tools.Json.Serializer.Serialize(resultValue), cancellationToken);
        }
    }
}