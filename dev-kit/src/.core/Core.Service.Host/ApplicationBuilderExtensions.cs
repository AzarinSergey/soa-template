using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Service.Host
{
    //localhost:33399/exp-appname-svc/exampleservicestate/GetState
    //{
    //      "value": "Trololo",
    //      "ctx":
    //      {
    //          "CrossContext":
    //          {
    //              "Uuid": "gh57894hgo9wgyh8934gonv8934gh8934"
    //          }
    //      }
    //}
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseServiceEndpoint<TService>(this IApplicationBuilder app, TService instance)
        where TService : class
        {
            var serviceType = typeof(TService);
            if(!serviceType.IsInterface)
                throw new ArgumentException($"'TService' - interface type only allowed.");

            var serviceName = serviceType.Name.StartsWith("I")
                ? new string(serviceType.Name.Skip(1).ToArray())
                : serviceType.Name;

            var methods = serviceType.GetMethods();

            app.UseEndpoints(builder =>
            {
                foreach (var methodInfo in methods)
                {
                    builder.MapPost($"{serviceName}/{methodInfo.Name}", async context =>
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
                                requestParams[i] = JsonSerializer.Deserialize(element.GetRawText(), methodParams[i].ParameterType);
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

                        await context.Response.WriteAsync(JsonSerializer.Serialize(resultValue), cancellationToken);
                    });
                }
            });

            return app;
        }
    }
}