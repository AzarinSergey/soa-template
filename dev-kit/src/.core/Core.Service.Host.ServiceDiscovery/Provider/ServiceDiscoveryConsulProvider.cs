using Consul;
using Core.Messages;
using Core.Service.Host.ServiceDiscovery.Interfaces;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Service.Host.ServiceDiscovery.Provider
{
    public class ServiceDiscoveryConsulProvider : IServiceDiscoveryProvider, IServiceEndpointConvention
    {
        private readonly ServiceDiscoveryConfig _currentServiceConfig;
        private readonly ConsulClient _client;

        private readonly Uri _currentServiceUri;
        private readonly string _currentServiceId;

        public ServiceDiscoveryConsulProvider(ServiceDiscoveryConfig config)
        {
            _currentServiceConfig = config ?? throw new ArgumentException(nameof(config));

            _client = new ConsulClient(consulConfig =>
            {
                consulConfig.Address = new Uri(config.ServiceDiscoveryAddress);
            });

            _currentServiceUri = new Uri(_currentServiceConfig.ServiceAddress);
            _currentServiceId = $"{_currentServiceConfig.ServiceId}-{_currentServiceUri.Port}";
        }


        public async Task RegisterService(string healthCheckPath, CancellationToken token = default)
        {
            var dnsIpAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .First(o => o.AddressFamily == AddressFamily.InterNetwork).ToString();

            var registrationModel = new AgentServiceRegistration
            {
                ID = _currentServiceId,
                Name = _currentServiceConfig.ServiceName,
                Address = dnsIpAddress,
                Port = _currentServiceUri.Port,
                Tags = _currentServiceConfig.Tags,
                Check = new AgentServiceCheck
                {
                    Timeout = TimeSpan.FromSeconds(3),
                    Interval = TimeSpan.FromSeconds(10),
                    HTTP = $"{_currentServiceUri.Scheme}://{dnsIpAddress}:{_currentServiceUri.Port}{healthCheckPath}",
                    Status = HealthStatus.Critical
                }
            };

            await _client.Agent.ServiceDeregister(registrationModel.ID, token).ConfigureAwait(false);
            await _client.Agent.ServiceRegister(registrationModel, token).ConfigureAwait(false);
        }

        public async Task UnregisterService(CancellationToken token = default)
        {
            await _client.Agent.ServiceDeregister(_currentServiceId).ConfigureAwait(false);
        }

        public Task AddEndpoints(Type[] contractInterfaces, CancellationToken token = default)
        {
            return Task.WhenAll(contractInterfaces
                .Select(contractInterface =>
                {
                    var serviceKey = GetServiceEndpointKey(contractInterface);
                    var serviceEndpointPrefixPath = GetServiceEndpointUri(_currentServiceConfig.ServiceName, contractInterface);

                    return _client.KV.Put(new KVPair(serviceKey)
                    {
                        Value = Encoding.UTF8.GetBytes(serviceEndpointPrefixPath)
                    }, token);
                }));
        }

        public async Task<string> GetServiceEndpointUri(Type contractInterface, CancellationToken token = default)
        {
            var result = await _client.KV.Get(GetServiceEndpointKey(contractInterface), token)
                .ConfigureAwait(false);

            return Encoding.UTF8.GetString(result.Response.Value);
        }

        public Task RemoveEndpointPrefixes(Type[] contractInterfaces, CancellationToken token = default)
        {
            return Task.WhenAll(contractInterfaces
                .Select(contractInterface => _client.KV.Delete(GetServiceEndpointKey(contractInterface), token)));
        }

        #region IServiceEndpointKeyConvention

        //TODO: move implementation from this?
        public string GetServiceEndpointKey(Type serviceInterfaceType)
        {
            var versionAttribute = GetVersionAttribute(serviceInterfaceType);

            var serviceTypeName = serviceInterfaceType.Name.StartsWith("I")
                ? new string(serviceInterfaceType.Name.Skip(1).ToArray())
                : serviceInterfaceType.Name;

            return $"{serviceTypeName}/v{versionAttribute.Full}/endpoint";
        }

        public string GetServiceEndpointUri(string serviceName, Type serviceInterfaceType)
        {
            var versionAttribute = GetVersionAttribute(serviceInterfaceType);

            var serviceTypeName = serviceInterfaceType.Name.StartsWith("I")
                ? new string(serviceInterfaceType.Name.Skip(1).ToArray())
                : serviceInterfaceType.Name;

            return $"{serviceName}/{serviceTypeName}/v{versionAttribute.Major}";
        }

        public string GetServiceKey(Type serviceInterfaceType)
        {
            return serviceInterfaceType.Name.StartsWith("I")
                ? new string(serviceInterfaceType.Name.Skip(1).ToArray())
                : serviceInterfaceType.Name;
        }

        private ContractApiVersionAttribute GetVersionAttribute(Type serviceInterfaceType)
        {
            if (!serviceInterfaceType.IsInterface)
                throw new ArgumentException($"Interface type only allowed to Consul KV registration.");

            var versionAttribute =
                (ContractApiVersionAttribute)Attribute.GetCustomAttribute(serviceInterfaceType, typeof(ContractApiVersionAttribute));

            if (versionAttribute == null)
                return new ContractApiVersionAttribute(0, 0, "version-not-specified");

            return versionAttribute;
        }

        #endregion
    }
}
