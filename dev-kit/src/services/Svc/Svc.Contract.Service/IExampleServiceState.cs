using Core.Messages;
using Core.Service.Host.ServiceDiscovery.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Svc.Contract.Service
{
    [ContractApiVersion(1,0, "postfix")]
    public interface IExampleServiceState : IDiscoverableHttpService
    {
        Task<string> ProcessByteArray(byte[] file, CrossContext ctx, CancellationToken token);
        Task<string> ProcessArrayOfByteArray(IReadOnlyCollection<byte[]> file, CrossContext ctx, CancellationToken token);
        Task<CrossContext> ProcessComplexModel(ComplexModel file, CrossContext ctx, CancellationToken token);
        Task<ComplexModel> GetComplexModel(CrossContext ctx, CancellationToken token);
    }

    public class ComplexModel
    {
        public List<byte[]> Files { get; set; }

        public string Encoding { get; set; }

        public IReadOnlyDictionary<DateTimeKind, DateTime> Dates { get; set; }

        public EventResetMode? NullableEnumNull { get; set; }

        public EventResetMode NullableEnumValue { get; set; }
    }
}
