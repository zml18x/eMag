using Newtonsoft.Json;

namespace eMag.Infrastructure.Models
{
    public class InvoiceAttachment
    {
        [JsonProperty("order_id")]
        public int OrderId { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
    }
}