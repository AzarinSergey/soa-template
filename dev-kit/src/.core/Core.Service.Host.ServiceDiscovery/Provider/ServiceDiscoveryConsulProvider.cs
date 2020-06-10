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
    public class ServiceDiscoveryConsulProvider : IServiceDiscoveryProvider, IServiceEndpointKeyConvention
    {
        private readonly ServiceDiscoveryConfig _config;
        private readonly ConsulClient _client;

        private readonly Uri _serviceUri;
        private readonly string _serviceId;

        public ServiceDiscoveryConsulProvider(ServiceDiscoveryConfig config)
        {
            _config = config ?? throw new ArgumentException(nameof(config));

            _client = new ConsulClient(consulConfig =>
            {
                consulConfig.Address = new Uri(config.ServiceDiscoveryAddress);
            });

            _serviceUri = new Uri(_config.ServiceAddress);
            _serviceId = $"{_config.ServiceId}-{_serviceUri.Port}";
        }


        public async Task RegisterService(string healthCheckPath, CancellationToken token = default)
        {
            var dnsIpAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .First(o => o.AddressFamily == AddressFamily.InterNetwork).ToString();

            var registrationModel = new AgentServiceRegistration
            {
                ID = _serviceId,
                Name = _config.ServiceName,
                Address = dnsIpAddress,
                Port = _serviceUri.Port,
                Tags = _config.Tags,
                Check = new AgentServiceCheck
                {
                    Timeout = TimeSpan.FromSeconds(3),
                    Interval = TimeSpan.FromSeconds(10),
                    HTTP = $"{_serviceUri.Scheme}://{dnsIpAddress}:{_serviceUri.Port}{healthCheckPath}",
                    Status = HealthStatus.Critical
                }
            };

            await _client.Agent.ServiceDeregister(registrationModel.ID, token).ConfigureAwait(false);
            await _client.Agent.ServiceRegister(registrationModel, token).ConfigureAwait(false);
        }

        public async Task UnregisterService(CancellationToken token = default)
        {
            await _client.Agent.ServiceDeregister(_serviceId).ConfigureAwait(false);
        }

        public Task AddEndpointPrefixes(Type[] contractInterfaces, CancellationToken token = default)
        {
            return Task.WhenAll(contractInterfaces
                .Select(contractInterface =>
                {
                    var serviceKey = GetServiceKey(contractInterface);
                    var serviceEndpointPrefixPath = GetServiceEndpointPathPrefix(_config.ServiceName, contractInterface);

                    return _client.KV.Put(new KVPair(serviceKey)
                    {
                        Value = Encoding.UTF8.GetBytes(serviceEndpointPrefixPath)
                    }, token);
                }));
        }

        public async Task<string> GetEndpointPrefix(Type contractInterface, CancellationToken token = default)
        {
            var result = await _client.KV.Get(GetServiceKey(contractInterface), token)
                .ConfigureAwait(false);

            return Encoding.UTF8.GetString(result.Response.Value);
        }

        public Task RemoveEndpointPrefixes(Type[] contractInterfaces, CancellationToken token = default)
        {
            return Task.WhenAll(contractInterfaces
                .Select(contractInterface => _client.KV.Delete(GetServiceKey(contractInterface), token)));
        }

        //TODO: move implementation from this
        public string GetServiceKey(Type serviceInterfaceType)
        {
            if (!serviceInterfaceType.IsInterface)
                throw new ArgumentException($"Interface type only allowed to Consul KV registration.");

            var versionAttribute =
                (ContractApiVersionAttribute)Attribute.GetCustomAttribute(serviceInterfaceType, typeof(ContractApiVersionAttribute));

            if(versionAttribute == null)
                throw new CustomAttributeFormatException("'ContractApiVersionAttribute' - required for discovered attributes");

            var serviceTypeName = serviceInterfaceType.Name.StartsWith("I")
                ? new string(serviceInterfaceType.Name.Skip(1).ToArray())
                : serviceInterfaceType.Name;

            return $"{serviceTypeName}/v{versionAttribute.Full}";
        }

        public string GetServiceEndpointPathPrefix(string serviceName, Type serviceInterfaceType)
        {
            if (!serviceInterfaceType.IsInterface)
                throw new ArgumentException($"Interface type only allowed to Consul KV registration.");

            var versionAttribute =
                (ContractApiVersionAttribute)Attribute.GetCustomAttribute(serviceInterfaceType, typeof(ContractApiVersionAttribute));

            if (versionAttribute == null)
                throw new CustomAttributeFormatException("'ContractApiVersionAttribute' - required for discovered attributes");

            var serviceTypeName = serviceInterfaceType.Name.StartsWith("I")
                ? new string(serviceInterfaceType.Name.Skip(1).ToArray())
                : serviceInterfaceType.Name;

            return $"{serviceName}/{serviceTypeName}/v{versionAttribute.Major}";
        }
    }
}
