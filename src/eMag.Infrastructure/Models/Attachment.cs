using Newtonsoft.Json;

namespace eMag.Infrastructure.Models
{
    public class Attachment
    {
        [JsonProperty(PropertyName = "type")]
        public int Type { get; set; }
    }
}