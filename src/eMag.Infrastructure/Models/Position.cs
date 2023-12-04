using Newtonsoft.Json;

namespace eMag.Infrastructure.Models
{
    public class Position
    {
        [JsonProperty("product_id")]
        public int ProductId { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}