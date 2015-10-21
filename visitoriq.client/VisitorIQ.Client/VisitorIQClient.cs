namespace VisitorIQ.Client
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    /// <summary>
    /// A class representing a client to the <c>VisitorIQ</c> service.
    /// </summary>
    public class VisitorIQClient
    {
        /// <summary>
        /// Initializes a new instance of <see cref="VisitorIQClient"/>.
        /// </summary>
        public VisitorIQClient()
        {
            HostName = Environment.GetEnvironmentVariable("VisitorIQ_HostName");
            ClientId = Environment.GetEnvironmentVariable("VisitorIQ_ClientId");
        }

        /// <summary>
        /// Gets or sets the host name of the service.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets the client Id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Looks up the geolocation information associated with the specified host name or IP address as an asynchronous operation.
        /// </summary>
        /// <param name="hostNameOrIPAddress">The host name or IP address to look up.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the result of looking up the specified host name or IP address.
        /// </returns>
        public async Task<IPLookup> LookupAsync(string hostNameOrIPAddress)
        {
            string requestUri = string.Format(
                CultureInfo.InvariantCulture,
                "http://{0}/evts/landing/index.php?EVTSQuery=%3Cxml%3E%3Crequest%3E%3Cclient_id%3E{1}%3C/client_id%3E%3Cproduct_id%3ERTMV%3C/product_id%3E%3Ctype%3EA%3C/type%3E%3Ctag%3EYES%3C/tag%3E%3Cconsumer%3E%3CIP_address%3E{2}%3C/IP_address%3E%3C/consumer%3C/request%3E%3C/xml%3E",
                Uri.EscapeDataString(HostName),
                Uri.EscapeDataString(ClientId),
                Uri.EscapeDataString(hostNameOrIPAddress ?? string.Empty));

            using (var client = new HttpClient())
            {
                using (StringContent content = new StringContent(string.Empty))
                {
                    using (var response = await client.PostAsync(requestUri, content))
                    {
                        response.EnsureSuccessStatusCode();

                        XElement result = XElement.Parse(await response.Content.ReadAsStringAsync());

                        XElement root = result.Element("response");
                        XElement segmentMember = root.Element("segment_member");

                        return new IPLookup()
                        {
                            IPAddress = root.Element("ip_address").Value,
                            Latitude = double.Parse(segmentMember.Element("post_lati").Value) / 1000000,
                            Longitude = double.Parse(segmentMember.Element("post_long").Value) / 1000000,
                            City = segmentMember.Element("city").Value,
                            RegionCode = segmentMember.Element("state").Value,
                            PostalCode = segmentMember.Element("postal_code").Value,
                            CountryCode = segmentMember.Element("country_code").Value,
                        };
                    }
                }
            }
        }
    }
}
