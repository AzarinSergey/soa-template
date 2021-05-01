using System.Threading;
using System.Threading.Tasks;
using Core.Service.Interfaces;

namespace Bsp.Contract.Service
{
    public interface IBookShopService : IInternalHttpService
    {
        Task<string> Ping(CancellationToken token);
    }
}
