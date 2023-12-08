using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using System.Net.Http.Json;

namespace CookieFactory.Portal
{

    public class WebPubSubClient : IDisposable
    {
        private readonly string negotiatorFunctionUri;
        private readonly string accesskey;
        private readonly ClientWebSocket webSocket;

        public WebPubSubClient(string negotiatorFunctionUri, string accesskey = null)
        {
            this.negotiatorFunctionUri = negotiatorFunctionUri;
            this.accesskey = accesskey;
            webSocket = new ClientWebSocket();
        }

        private Task backgroundWorker;
        private CancellationTokenSource cts = new();

        public event Func<string, Task> OnMessage;

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, negotiatorFunctionUri);

            if (accesskey is not null)
                request.Headers.Add("x-functions-key", accesskey);

            var response = await httpClient.SendAsync(request, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);
            var webSocketUri = result.GetProperty("url").GetString();
            await webSocket.ConnectAsync(new Uri(webSocketUri), cancellationToken);
            backgroundWorker = RunAsync(cts.Token);
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);

            while (!cancellationToken.IsCancellationRequested)
            {
                WebSocketReceiveResult result;

                using var memoryStream = new MemoryStream();
                do
                {
                    result = await webSocket.ReceiveAsync(buffer, cancellationToken);
                    memoryStream.Write(buffer.Array, buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                memoryStream.Seek(0, SeekOrigin.Begin);

                var message = Encoding.UTF8.GetString(memoryStream.ToArray());

                if (OnMessage is not null)
                    await OnMessage.Invoke(message);
            }
        }

        public void Dispose()
        {
            cts.Cancel();
            webSocket.Dispose();
            cts.Dispose();
        }
    }
}
