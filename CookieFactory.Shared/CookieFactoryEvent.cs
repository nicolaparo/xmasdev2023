using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CookieFactory.Shared
{
    public class CookieFactoryEvent
    {
        public CookieFactoryEvent() { }
        public CookieFactoryEvent(CookieFactoryEventSeverity severity, string type, string message, object data) : this(severity, type, message, JsonSerializer.Serialize(data)) { }
        public CookieFactoryEvent(CookieFactoryEventSeverity severity, string type, string message, string data)
        {
            Severity = severity;
            Type = type;
            Message = message;
            Data = data;
        }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("severity")]
        public CookieFactoryEventSeverity Severity { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public string Data { get; set; }
    }
}
