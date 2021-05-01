using System.Threading;
using System.Threading.Tasks;
using Bsp.Contract.Service;
using Core.Service.Host.Client.DynamicProxy;
using Microsoft.AspNetCore.Mvc;

namespace Api.Test.Controllers
{
    public class BspController : ApiControllerBase
    {
        private ServiceProxy<IBookShopService> _proxy;

        public BspController(ServiceProxy<IBookShopService> serviceProxy)
        {
            _proxy = serviceProxy;
        }

        [HttpGet("ping")]
        public async Task<IActionResult> Ping(CancellationToken token)
        {
            var result = await _proxy.Call().Ping(token);
            return Ok(result);
        }
    }
}