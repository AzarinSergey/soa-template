using System.Threading;
using System.Threading.Tasks;
using Bsp.Contract.Service;

namespace Bsp.Implementation.Service
{
    public partial class BookShopService : IBookShopService
    {
        public Task<string> Ping(CancellationToken token)
        {
            return Task.FromResult("PONG");
        }
    }
}