using Core.Messages;
using Core.Service.Host.Client.DynamicProxy;
using Exs.Contract.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Tool;
using Exs.Contract.Service.Models;

namespace Api.Test.Controllers
{
    public class TestController : ApiControllerBase
    {
        private readonly ServiceProxy<IExampleServiceState> _proxy;

        public TestController(ServiceProxy<IExampleServiceState> serviceProxy)
        {
            _proxy = serviceProxy;
        }

        [HttpGet("all")]
        public async Task<IActionResult> All(CancellationToken token)
        {
            var reportBuilder = new StringBuilder();

            reportBuilder.AppendLine("\t\t*******************************\t\t***********************************");
            var request1 = (
                    Encoding.UTF8.GetBytes("Trolololol, ===> FILE CONTENT <===== kjdfnjk"),
                    new CrossContext { Uuid = Guid.NewGuid().ToString() }
                );
            var result1 = await _proxy.Call().ProcessByteArray(request1.Item1, request1.Item2, token);



            FormatResponse(reportBuilder, request1, result1);
            reportBuilder.AppendLine("\t\t*******************************\t\t***********************************");
            var request2 = (
                new List<byte[]> {
                            Encoding.UTF8.GetBytes("Kjhfeojgnfv, ===> FILE CONTENT <===== kjdfnjk"),
                            Encoding.UTF8.GetBytes("P:oaejfrjebvvf, ===> FILE CONTENT <===== kjdfnjk")
                },
                new CrossContext { Uuid = Guid.NewGuid().ToString() }
            );
            var result2 = await _proxy.Call().ProcessArrayOfByteArray(request2.Item1, request1.Item2, token);
            FormatResponse(reportBuilder, request2, result2);
            reportBuilder.AppendLine("\t\t*******************************\t\t***********************************");



            FormatResponse(reportBuilder, request1, result1);
            reportBuilder.AppendLine("\t\t*******************************\t\t***********************************");
            var request3 = (
                new CrossContext { Uuid = Guid.NewGuid().ToString() }
            );
            var result3 = await _proxy.Call().GetComplexModel(request1.Item2, token);
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
                                Encoding.UTF8.GetBytes("Совет. По этому запросу вы можете найти сайты на русском языке. Указать предпочтительные языки для результатов поиска можно в разделе Настройки."),
                                Encoding.UTF8.GetBytes("enough for me to be able to correlate the generated error with an HTTP request")
                    },
                    NullableEnumNull = null,
                    NullableEnumValue = EventResetMode.ManualReset
                },
                new CrossContext { Uuid = Guid.NewGuid().ToString() }
            );
            var result4 = await _proxy.Call().ProcessComplexModel(request4.Item1, request4.Item2, token);
            FormatResponse(reportBuilder, request4, result4);
            reportBuilder.AppendLine("\t\t*******************************\t\t***********************************");

            return Ok(reportBuilder.ToString());
        }

        private StringBuilder FormatResponse(StringBuilder builder, object request, object response)
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