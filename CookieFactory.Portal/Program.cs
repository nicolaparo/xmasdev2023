using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace CookieFactory.Portal
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            var webPubSubClient = new WebPubSubClient(@"https://xmasdev-receiver.azurewebsites.net/api/negotiate");
            await webPubSubClient.ConnectAsync(CancellationToken.None);

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddSingleton(webPubSubClient);

            await builder.Build().RunAsync();
        }
    }
}
