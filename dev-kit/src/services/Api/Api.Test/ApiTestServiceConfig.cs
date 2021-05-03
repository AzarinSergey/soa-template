using Core.Service.Host.Convention.Configuration;

namespace Api.Test
{
    public class ApiTestServiceConfig : ServiceConfig
    {
        public string RedisConnection { get; set; }
        public string PostgresConnection { get; set; }
    }
}