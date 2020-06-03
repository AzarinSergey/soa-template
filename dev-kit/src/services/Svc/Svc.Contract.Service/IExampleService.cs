using System.Threading;
using System.Threading.Tasks;
using Svc.Implementation;

namespace Svc.Contract.Service
{
    public interface IExampleServiceState
    {
        Task WriteState(string value, CrossContext ctx, CancellationToken token);
        Task<string> GetState(string value, CrossContext ctx, CancellationToken token);
        Task<string> UpdateState(string value, CrossContext ctx, CancellationToken token);
    }
}
