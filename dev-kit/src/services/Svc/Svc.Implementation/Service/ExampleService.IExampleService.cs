using System.Threading;
using System.Threading.Tasks;
using Core.Messages;
using Svc.Contract.Service;

namespace Svc.Implementation.Service
{
    public partial class ExampleService : IExampleServiceState
    {
        public Task<string> UpdateState(int id, string value, CrossContext ctx, CancellationToken token)
        {
            return Task.FromResult("public Task<string> UpdateState(int id, string value, CrossContext ctx, CancellationToken token)");
        }

        public Task<int> AddState(string value, CrossContext ctx, CancellationToken token)
        {
            return Task.FromResult(1);
        }

        //localhost:33399/exp-appname-svc/exampleservicestate/getstate
        //{
        //      "id": 123,
        //      "ctx":
        //      {
        //          "CrossContext":
        //          {
        //              "Uuid": "gh57894hgo9wgyh8934gonv8934gh8934"
        //          }
        //      }
        //}
        public Task<string> GetState(int? id, CrossContext ctx, CancellationToken token)
        {
            return Task.FromResult("public Task<string> GetState(int? id, CrossContext ctx, CancellationToken token)");
        }
    }
}
