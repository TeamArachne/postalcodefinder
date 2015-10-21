namespace FreeGeoIP.Client
{
    using Newtonsoft.Json;

    /// <summary>
    /// A class representing the result of a <c>https://freegeoip.net</c> IP lookup. This class cannot be inherited.
    /// </summary>
    public sealed class IPLookup
    {
        /// <summary>
        /// Gets or sets the matched IP address.
        /// </summary>
        [JsonProperty("ip")]
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the country code associated with the IP address.
        /// </summary>
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the country name associated with the IP address.
        /// </summary>
        [JsonProperty("country_name")]
        public string CountryName { get; set; }

        /// <summary>
        /// Gets or sets the region code associated with the IP address.
        /// </summary>
        [JsonProperty("region_code")]
        public string RegionCode { get; set; }

        /// <summary>
        /// Gets or sets the region name associated with the IP address.
        /// </summary>
        [JsonProperty("region_name")]
        public string RegionName { get; set; }

        /// <summary>
        /// Gets or sets the city associated with the IP address.
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the postal code associated with the IP address.
        /// </summary>
        [JsonProperty("zip_code")]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the time zone associated with the IP address.
        /// </summary>
        [JsonProperty("time_zone")]
        public string TimeZone { get; set; }

        /// <summary>
        /// Gets or sets the latitude associated with the IP address.
        /// </summary>
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude associated with the IP address.
        /// </summary>
        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        /// <summary>
        /// Gets or sets the metro code associated with the IP address.
        /// </summary>
        [JsonProperty("metro_code")]
        public double MetroCode { get; set; }
    }
}
