using System.Net.WebSockets;
using System.Text.Json;
using System.Text;

namespace CookieFactory.Portal
{
    public class WebSocketClient : IDisposable
    {
        private readonly ClientWebSocket webSocket;
        private readonly string webSocketUri;

        public WebSocketClient(string webSocketUri)
        {
            this.webSocket = new ClientWebSocket();
            this.webSocketUri = webSocketUri;
        }

        private Task backgroundWorker;
        private CancellationTokenSource cts = new();

        public event Func<string, Task> OnMessage;

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            
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
