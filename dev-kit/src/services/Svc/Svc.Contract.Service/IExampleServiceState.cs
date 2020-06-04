using Core.Service.Host.ServiceDiscovery.Interfaces;
using Svc.Implementation;
using System.Threading;
using System.Threading.Tasks;

namespace Svc.Contract.Service
{
    public interface IExampleServiceState : IDiscoverableHttpService
    {
        Task<string> UpdateState(int id, string value, CrossContext ctx, CancellationToken token);
        Task<int> AddState(string value, CrossContext ctx, CancellationToken token);
        Task<string> GetState(int? id, CrossContext ctx, CancellationToken token);
    }
}
