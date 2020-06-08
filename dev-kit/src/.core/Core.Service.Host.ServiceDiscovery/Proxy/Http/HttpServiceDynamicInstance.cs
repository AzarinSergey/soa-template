using System.Net.Http;
using System.Threading.Tasks;

namespace Core.Service.Host.ServiceDiscovery.Proxy.Http
{
    internal interface IHttpServiceDynamicInstance
    {
        Task CallAsync(string path, HttpContent content);
        Task<T> CallAsync<T>(string path, HttpContent content);
    }
    internal class HttpServiceDynamicInstance : IHttpServiceDynamicInstance
    {
        private readonly HttpClient _client;

        public HttpServiceDynamicInstance(HttpClient client)
        {
            _client = client;
        }

        public async Task CallAsync(string path, HttpContent content)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, path) { Content = content };

            await _client.SendAsync(request);
        }

        public async Task<T> CallAsync<T>(string path, HttpContent content)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, path) { Content = content };

            var result = await _client.SendAsync(request);

            return System.Text.Json.JsonSerializer.Deserialize<T>(await result.Content.ReadAsStringAsync());
        }
    }
}