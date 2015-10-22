namespace Google.Geocoding.Client
{
    /// <summary>
    /// A class representing the result of looking up the address for a latitude and longitude using the Google Maps Geocoding API.
    /// </summary>
    public class CoordinateLookup
    {
        /// <summary>
        /// Gets or sets the country code associated with the coordinates.
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the region code associated with the coordinates.
        /// </summary>
        public string RegionCode { get; set; }

        /// <summary>
        /// Gets or sets the city associated with the coordinates.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the postal code associated with the coordinates.
        /// </summary>
        public string PostalCode { get; set; }
    }
}
