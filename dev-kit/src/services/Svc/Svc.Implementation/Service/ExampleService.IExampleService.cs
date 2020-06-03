using System.Threading;
using System.Threading.Tasks;
using Svc.Contract.Service;

namespace Svc.Implementation.Service
{
    public partial class ExampleService : IExampleServiceState
    {
        public Task WriteState(string value, CrossContext ctx, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        public Task<string> GetState(string value, CrossContext ctx, CancellationToken token)
        {
            return Task.FromResult("public Task<string> GetState(string value, CrossContext ctx, CancellationToken token)");
        }

        public Task<string> UpdateState(string value, CrossContext ctx, CancellationToken token)
        {
            return Task.FromResult("public Task<string> UpdateState(string value, CrossContext ctx, CancellationToken token)");
        }
    }
}
