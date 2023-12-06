using System.Text.Json;
using System.Text.Json.Serialization;

namespace CookieFactory.Shared
{
    public class CloudEvent
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("specversion")]
        public string SpecVersion => "1.0";

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("data")]
        public JsonElement Data { get; set; }

        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        [JsonPropertyName("datacontenttype")]
        public string DataContentType { get; set; }

        [JsonPropertyName("subject")]
        public string Subject { get; set; }

    }
}
