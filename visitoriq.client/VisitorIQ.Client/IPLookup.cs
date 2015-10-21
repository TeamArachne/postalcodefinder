namespace VisitorIQ.Client
{
    /// <summary>
    /// A class representing the result of a <c>VisitorIQ</c> IP lookup. This class cannot be inherited.
    /// </summary>
    public sealed class IPLookup
    {
        /// <summary>
        /// Gets or sets the matched IP address.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the country code associated with the IP address.
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the region code associated with the IP address.
        /// </summary>
        public string RegionCode { get; set; }

        /// <summary>
        /// Gets or sets the city associated with the IP address.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the postal code associated with the IP address.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the latitude associated with the IP address.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude associated with the IP address.
        /// </summary>
        public double Longitude { get; set; }
    }
}
