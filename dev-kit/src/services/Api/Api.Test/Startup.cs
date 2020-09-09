using Cmn.Constants;
using Core.Messages;
using Core.Service.Host;
using Core.Service.Host.ServiceDiscovery;
using Core.Service.Host.ServiceDiscovery.DynamicProxy;
using Core.Service.Host.ServiceDiscovery.Interfaces;
using Core.Tool;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Svc.Contract.Service;
using Svc.Contract.Service.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Api.Test
{
    public class Startup : StatelessServiceStartup
    {
        protected override Type[] ServiceContractTypes => Array.Empty<Type>();

        public override void RegisterStatelessService(IServiceCollection services)
        {
            services.AddHttpClient<ServiceProxy<IExampleServiceState>>((provider, client) =>
            {
                var cfg = provider.GetRequiredService<IOptions<ApplicationConfig>>().Value;
                var convention = provider.GetRequiredService<IServiceEndpointConvention>();
                client.BaseAddress = new Uri($"http://{convention.GetServiceHost(cfg, ServiceNames.BackendExample)}");
            });
        }

        public override void ServiceConfiguration(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //TODO: �������� MVC �������
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/all", async context =>
                {
                    var proxy = app.ApplicationServices.GetService<ServiceProxy<IExampleServiceState>>();

                    var reportBuilder = new StringBuilder();

                    reportBuilder.AppendLine("\t\t*******************************\t\t***********************************");
                    var request1 = (
                            Encoding.UTF8.GetBytes("Trolololol, ===> FILE CONTENT <===== kjdfnjk"),
                            new CrossContext { Uuid = Guid.NewGuid().ToString() }
                        );
                    var result1 = await proxy.Call().ProcessByteArray(request1.Item1, request1.Item2, context.RequestAborted);



                    FormatResponse(reportBuilder, request1, result1);
                    reportBuilder.AppendLine("\t\t*******************************\t\t***********************************");
                    var request2 = (
                        new List<byte[]> {
                            Encoding.UTF8.GetBytes("Kjhfeojgnfv, ===> FILE CONTENT <===== kjdfnjk"),
                            Encoding.UTF8.GetBytes("P:oaejfrjebvvf, ===> FILE CONTENT <===== kjdfnjk")
                        },
                        new CrossContext { Uuid = Guid.NewGuid().ToString() }
                    );
                    var result2 = await proxy.Call().ProcessArrayOfByteArray(request2.Item1, request1.Item2, context.RequestAborted);
                    FormatResponse(reportBuilder, request2, result2);
                    reportBuilder.AppendLine("\t\t*******************************\t\t***********************************");



                    FormatResponse(reportBuilder, request1, result1);
                    reportBuilder.AppendLine("\t\t*******************************\t\t***********************************");
                    var request3 = (
                        new CrossContext { Uuid = Guid.NewGuid().ToString() }
                    );
                    var result3 = await proxy.Call().GetComplexModel(request1.Item2, context.RequestAborted);
                    FormatResponse(reportBuilder, request3, result3);
                    reportBuilder.AppendLine("\t\t*******************************\t\t***********************************");

                    FormatResponse(reportBuilder, request1, result1);
                    reportBuilder.AppendLine("\t\t*******************************\t\t***********************************");
                    var request4 = (
                        new ComplexModel
                        {
                            Encoding = Encoding.UTF8.BodyName,
                            Dates = new Dictionary<DateTimeKind, DateTime>
                            {
                                { DateTimeKind.Local, DateTime.Now },
                                { DateTimeKind.Utc, DateTime.UtcNow },
                                { DateTimeKind.Unspecified, DateTime.UnixEpoch }
                            },
                            Files = new List<byte[]>
                            {
                                Encoding.UTF8.GetBytes("2dcf2b49-87b8-451e-8f70-dd8b0ba5d610"),
                                Encoding.UTF8.GetBytes("�����. �� ����� ������� �� ������ ����� ����� �� ������� �����. ������� ���������������� ����� ��� ����������� ������ ����� � ������� ���������."),
                                Encoding.UTF8.GetBytes("enough for me to be able to correlate the generated error with an HTTP request")
                            },
                            NullableEnumNull = null,
                            NullableEnumValue = EventResetMode.ManualReset
                        },
                        new CrossContext { Uuid = Guid.NewGuid().ToString() }
                    );
                    var result4 = await proxy.Call().ProcessComplexModel(request4.Item1, request4.Item2, context.RequestAborted);
                    FormatResponse(reportBuilder, request4, result4);
                    reportBuilder.AppendLine("\t\t*******************************\t\t***********************************");

                    await context.Response.WriteAsync($"Response value: \n {reportBuilder}");
                });
            });
        }

        public StringBuilder FormatResponse(StringBuilder builder, object request, object response)
        {
            builder.AppendLine("<b>REQUEST DATA:</b>");
            builder.AppendLine("<b>REQUEST DATA:</b>");
            builder.AppendLine(Tools.Json.Serializer.Serialize(request));
            builder.AppendLine("<b>RESPONSE DATA:</b>");
            builder.AppendLine(Tools.Json.Serializer.Serialize(response));

            return builder;
        }
    }
}