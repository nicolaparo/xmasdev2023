using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CookieFactory.Collector.Models
{
    public class InventoryEntry
    {
        [JsonPropertyName("Slot")]
        public int Slot { get; set; }

        [JsonPropertyName("id")]
        public string ItemId { get; set; }

        [JsonPropertyName("Count")]
        public int Count { get; set; }
    }

}
