using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Service.Host.ServiceDiscovery.Interfaces
{
    public interface IServiceDiscoveryProvider
    {
        Task RegisterService(string healthCheckPath, CancellationToken token = default);

        Task UnregisterService(CancellationToken token = default);

        Task AddEndpointPrefixes(Type[] contractInterface, CancellationToken token = default);

        Task<string> GetEndpointPrefix(Type contractInterface, CancellationToken token = default);

        Task RemoveEndpointPrefixes(Type[] contractInterfaces, CancellationToken token = default);
    }
}
