using Newtonsoft.Json;

namespace eMag.Infrastructure.Models
{
    public class Customer
    {
        [JsonProperty("id")]
        public int CustomerId { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("company")]
        public string CompanyName { get; set; }
        [JsonProperty("registration_number")]
        public string CompanyRegistrationNumber { get; set; }
        [JsonProperty("iban")]
        public string Iban { get; set; }
        [JsonProperty("is_vat_payer")]
        public int IsVatPayer { get; set; }
        [JsonProperty("billing_name")]
        public string BillingName { get; set; }
        [JsonProperty("billing_phone")]
        public string Phone { get; set; }
        [JsonProperty("billing_country")]
        public string Country { get; set; }
        [JsonProperty("billing_suburb")]
        public string Suburb { get; set; }
        [JsonProperty("billing_city")]
        public string City { get; set; }
        [JsonProperty("billing_street")]
        public string Street { get; set; }
        [JsonProperty("billing_postal_code")]
        public string PostalCode { get; set; }
    }
}