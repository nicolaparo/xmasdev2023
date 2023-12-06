using CookieFactory.Shared;
using System.Net.Http.Json;
using System.Text.Json;

namespace CookieFactory.Collector
{
    public class NotifierService(string notificationEndpointUrl) : IDisposable
    {
        private readonly HttpClient client = new();

        public void NotifyAsync(CookieFactoryEvent evt)
        {
            client.PostAsJsonAsync(notificationEndpointUrl, new CloudEvent
            {
                Source = "CookieFactory.Collector",
                Type = evt.Type,
                Time = evt.Timestamp,
                Data = JsonSerializer.SerializeToElement(evt)
            });

            Console.ForegroundColor = evt.Severity switch
            {
                CookieFactoryEventSeverity.Info => ConsoleColor.Blue,
                CookieFactoryEventSeverity.Success => ConsoleColor.Green,
                CookieFactoryEventSeverity.Warning => ConsoleColor.Yellow,
                CookieFactoryEventSeverity.Error => ConsoleColor.Red,
                _ => throw new NotImplementedException()
            };
            Console.WriteLine($"{evt.Type} - {evt.Message} - Data: {evt.Data}");
            Console.ResetColor();
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
