using Core.Messages;
using Core.Tool;
using Svc.Contract.Service;
using Svc.Contract.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Svc.Implementation.Service
{
    public partial class ExampleService : IExampleServiceState
    {
        public Task<string> Ping(CancellationToken token)
        {
            return Task.FromResult("PONG");
        }

        public Task<string> ProcessByteArray(byte[] file, CrossContext ctx, CancellationToken token)
        {
            return Task.FromResult(Encoding.UTF8.GetString(file));
        }

        public Task<string> ProcessArrayOfByteArray(IReadOnlyCollection<byte[]> file, CrossContext ctx, CancellationToken token)
        {
            return Task.FromResult(Tools.Json.Serializer.Serialize(file.Select(x => Encoding.UTF8.GetString(x))));
        }

        public Task<CrossContext> ProcessComplexModel(ComplexModel file, CrossContext ctx, CancellationToken token)
        {
            return Task.FromResult(ctx);
        }

        public Task<ComplexModel> GetComplexModel(CrossContext ctx, CancellationToken token)
        {
            return Task.FromResult(new ComplexModel {
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
                    Encoding.UTF8.GetBytes("Совет. По этому запросу вы можете найти сайты на русском языке. Указать языки для результатов поиска можно в разделе Настройки."),
                    Encoding.UTF8.GetBytes("enough for me to be able to correlate the generated error with an HTTP request")
                },
                NullableEnumNull = null,
                NullableEnumValue = EventResetMode.ManualReset
            });
        }
    }
}
