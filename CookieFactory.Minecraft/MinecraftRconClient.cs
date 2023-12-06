using System.Net.Sockets;
using System.Text;

namespace CookieFactory.Minecraft
{
    public class MinecraftRconClient : IDisposable
    {
        private const int MaxMessageSize = 4110;

        private TcpClient client;
        private NetworkStream connection;
        private int lastMessageId = 0;

        public MinecraftRconClient(string host, int port)
        {
            client = new TcpClient(host, port);
            connection = client.GetStream();
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            connection.Close();
            client.Close();
        }

        public async Task<bool> AuthenticateAsync(string password)
        {
            var result = await sendMessageAsync(new Message(
                password.Length + MinecraftMessageSerializer.HeaderLength,
                Interlocked.Increment(ref lastMessageId),
                MessageType.Authenticate,
                password
            ));

            return result.Success;
        }

        public async Task<MessageResponse> SendCommandAsync(string command)
        {
            return await sendMessageAsync(new Message(
                command.Length + MinecraftMessageSerializer.HeaderLength,
                Interlocked.Increment(ref lastMessageId),
                MessageType.Command,
                command
            ));
        }

        private async Task<MessageResponse> sendMessageAsync(Message req)
        {
            byte[] encoded = MinecraftMessageSerializer.Serialize(req);

            await connection.WriteAsync(encoded, 0, encoded.Length);
            await connection.FlushAsync();

            var responses = new List<Message>();

            do
            {
                byte[] respBytes = new byte[MaxMessageSize];
                int bytesRead = await connection.ReadAsync(respBytes, 0, respBytes.Length);
                Array.Resize(ref respBytes, bytesRead);
                responses.Add(MinecraftMessageSerializer.Deserialize(respBytes));
            }
            while (connection.DataAvailable);

            if (responses is [var resp])
                return new MessageResponse(resp, req.Id == resp.Id);

            return new MessageResponse(
                new Message(responses.Sum(r => r.Length), req.Id, MessageType.Response, string.Join("", responses.Select(r => r.Body))),
                true
            );
        }

    }

    public enum MessageType : int
    {
        Response = 0,
        Command = 2,
        Authenticate = 3
    }

    public record MessageResponse(Message Message, bool Success);
    public record Message(int Length, int Id, MessageType Type, string Body);

    public class MinecraftMessageSerializer
    {
        public const int HeaderLength = 10; // Does not include 4-byte message length.

        public static byte[] Serialize(Message msg)
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(BitConverter.GetBytes(msg.Length));
            bytes.AddRange(BitConverter.GetBytes(msg.Id));
            bytes.AddRange(BitConverter.GetBytes((int)msg.Type));
            bytes.AddRange(Encoding.ASCII.GetBytes(msg.Body));
            bytes.AddRange(new byte[] { 0, 0 });

            return bytes.ToArray();
        }

        public static Message Deserialize(byte[] rawData)
        {
            var messageLength = BitConverter.ToInt32(rawData, 0);
            var messageId = BitConverter.ToInt32(rawData, 4);
            var messageType = BitConverter.ToInt32(rawData, 8);

            var messageBodyLength = rawData.Length - (HeaderLength + 4);
            if (messageBodyLength > 0)
            {
                byte[] bodyBytes = new byte[messageBodyLength];
                Array.Copy(rawData, 12, bodyBytes, 0, messageBodyLength);
                Array.Resize(ref bodyBytes, messageBodyLength);
                return new Message(messageLength, messageId, (MessageType)messageType, Encoding.ASCII.GetString(bodyBytes));
            }
            else
            {
                return new Message(messageLength, messageId, (MessageType)messageType, "");
            }
        }
    }
}