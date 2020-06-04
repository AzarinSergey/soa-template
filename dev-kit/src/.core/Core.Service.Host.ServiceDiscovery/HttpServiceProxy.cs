using Consul;
using Core.Service.Host.ServiceDiscovery.Interfaces;
using System;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service.Host.ServiceDiscovery
{
    public static class HttpServiceProxy
    {
        public static IConsulClient Client;
        public static string ReverseProxyAddress;

        public static async Task<TResult> Call<TService, TResult>(Expression<Func<TService, Task<TResult>>> callService)
            where TService : IDiscoverableHttpService
            where TResult : class
        {
            var serviceType = typeof(TService);
            if (!serviceType.IsInterface)
                throw new ArgumentException($"'TService' - interface type only allowed.");

            var getPair = await Client.KV.Get(serviceType.FullName);
            var serviceInterfaceUrl = $"{ReverseProxyAddress}" +
                                      $"/" +
                                      $"{Encoding.UTF8.GetString(getPair.Response.Value, 0,getPair.Response.Value.Length)}";


            //TODO: реализовать разбор выражения и сделать соответствующий вызов 

            return null;
        }
    }
}