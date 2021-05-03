using Microsoft.Extensions.Hosting;
using Orleans.Hosting;
using System.Threading.Tasks;

namespace Silo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .UseOrleans((context, siloBuilder) =>
            {
                siloBuilder.UseKubernetesHosting();

                //var redisConnectionString = $"{Environment.GetEnvironmentVariable("REDIS")}:6379";
                siloBuilder.UseRedisClustering(options =>
                {
                    options.ConnectionString = "";
                });
                siloBuilder.AddRedisGrainStorage("definitions", options =>
                {
                    options.ConnectionString = "";
                });

            }).RunConsoleAsync();
        }
    }
}
