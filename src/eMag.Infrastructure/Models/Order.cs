using Newtonsoft.Json;
using System.Net.Mail;

namespace eMag.Infrastructure.Models
{
    public class Order
    {
        [JsonProperty("id")]
        public int OrderId { get; set; }
        [JsonProperty("date")]
        public string SellDate { get; set; }
        [JsonProperty("payment_mode_id")]
        public int PaymentMode { get; set; }
        [JsonProperty("payment_status")]
        public int PaymentStatus { get; set; }
        [JsonProperty("customer")]
        public Customer CustomerDetails { get; set; }
        [JsonProperty("products")]
        public List<Product> Positions { get; set; }
        [JsonProperty("attachments")]
        public List<Attachment> Attachments { get; set; }
        public string ApiUrl { get; set; }
    }
}