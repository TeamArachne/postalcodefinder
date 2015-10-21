namespace FreeGeoIP.Client
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// A class representing a client for the <c>https://freegeoip.net</c> service.
    /// </summary>
    public class FreeGeoIPClient
    {
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
                "https://freegeoip.net/json/{0}",
                Uri.EscapeDataString(hostNameOrIPAddress));

            using (HttpClient client = new HttpClient())
            {
                using (var response = await client.GetAsync(requestUri))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsAsync<IPLookup>();
                }
            }
        }
    }
}
