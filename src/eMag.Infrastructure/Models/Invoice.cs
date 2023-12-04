using Newtonsoft.Json;

namespace eMag.Infrastructure.Models
{
    public class Invoice
    {
        [JsonProperty("invoice_id")]
        public int InvoiceId { get; set; }
        [JsonProperty("kind")]
        public string Kind { get; set; }
        [JsonProperty("income")]
        public int Income { get; set; }
        [JsonProperty("sell_date")]
        public string SellDate { get; set; }
        [JsonProperty("payment_to_kind")]
        public int PaymentToKind { get; set; }
        [JsonProperty("client_id")]
        public int CustomerId { get; set; }
        [JsonProperty("positions")]
        public List<Product> Positions { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("lang")]
        public string Language { get; set; }
        public string ApiUrl { get; set; }
    }
}