using eMag.Infrastructure.Models;
using Newtonsoft.Json;

namespace eMag.Infrastructure.Requests
{
    public class PostInvoiceRequest
    {
        [JsonProperty("api_token")]
        public string ApiToken { get; set; }
        [JsonProperty("invoice")]
        public Invoice Invoice { get; set; }



        public PostInvoiceRequest(string apiToken, Invoice invoice)
        {
            ApiToken = apiToken;
            Invoice = invoice;
        }
    }
}