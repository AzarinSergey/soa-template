using Core.Service.Host.ServiceDiscovery;
using Microsoft.AspNetCore.Mvc;
using Svc.Contract.Service;
using Svc.Implementation;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Test.Controllers
{
    public class ExampleServiceStateController : Controller
    {
        [HttpGet("state")]
        public async Task<IActionResult> Get(CancellationToken token)
        {
            var result = await HttpServiceProxy
                .Call<IExampleServiceState, string>(svc => svc.GetState(1, new CrossContext(), token));

            return Ok($"Response value: {result}");
        }
    }
}