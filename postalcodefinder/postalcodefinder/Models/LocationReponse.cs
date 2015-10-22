using Newtonsoft.Json;

namespace postalcodefinder.Models
{
    public sealed class LocationReponse
    {
        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("derivationMethod")]
        public string DerivationMethod { get; set; }
    }
}
