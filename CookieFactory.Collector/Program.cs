using CookieFactory.Minecraft;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace CookieFactory.Collector
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.local.json");

            var config = configuration.Build();

            var client = new MinecraftRconClient(config["RconHost"], Convert.ToInt32(config["RconPort"]));
            await client.AuthenticateAsync(config["RconPassword"]);

            var notifier = new NotifierService(config["NotifierUri"]);
            var reader = new CookieFactoryDataReader(client);
            var monitor = new CookieFactoryMonitorService(reader, notifier);

            Console.CancelKeyPress += (s, e) => monitor.StopAsync();

            await monitor.RunAsync();
        }
    }
}
