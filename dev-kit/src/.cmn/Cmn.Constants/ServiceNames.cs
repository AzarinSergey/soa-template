namespace Cmn.Constants
{
    /// <summary>
    /// Check service name inside pod of service with command 'printenv',
    /// after Kubebridge was started at least once for this service
    /// </summary>
    public class ServiceNames
    {
        public const string BackendExample = "exs";
        public const string ApiTest = "api-test";
        public const string BookShop = "bsp";
    }
}
