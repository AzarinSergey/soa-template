namespace Core.Service.Host.ServiceDiscovery
{
    public class ServiceDiscoveryConfig
    {
        public string ServiceAddress { get; set; }

        public string ServiceId { get; set; }

        public string ServiceName { get; set; }

        public string[] Tags { get; set; }

        public string ReverseProxyAddress { get; set; }
    }
}