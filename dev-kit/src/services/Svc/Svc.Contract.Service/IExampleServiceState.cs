using Core.Service.Host.ServiceDiscovery.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using Core.Messages;

namespace Svc.Contract.Service
{
    [ContractApiVersion(1,0, "postfix")]
    public interface IExampleServiceState : IDiscoverableHttpService
    {
        Task<string> UpdateState(int id, string value, CrossContext ctx, CancellationToken token);
        Task<int> AddState(string value, CrossContext ctx, CancellationToken token);
        Task<string> GetState(int? id, CrossContext ctx, CancellationToken token);
    }
}
