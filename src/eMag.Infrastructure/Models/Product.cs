using Newtonsoft.Json;

namespace eMag.Infrastructure.Models
{
    public class Product
    {
        [JsonProperty("product_id")]
        public int ProductId { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
    }
}