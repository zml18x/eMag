using Newtonsoft.Json;

namespace eMag.Infrastructure.Models
{
    public class InvoicePdf
    {
        [JsonProperty("token")]
        public string InvoiceToken { get; set; }
        public int OrderId { get; set; }
        public string ApiUrl { get; set; }
        public string InvoiceUrl { get; set; }

    }
}