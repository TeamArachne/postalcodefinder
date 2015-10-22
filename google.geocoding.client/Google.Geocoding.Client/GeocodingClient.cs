namespace Google.Geocoding.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    /// <summary>
    /// A class representing a client for the Google Maps geocoding API.
    /// </summary>
    public class GeocodingClient
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GeocodingClient"/>.
        /// </summary>
        public GeocodingClient()
        {
            ApiKey = Environment.GetEnvironmentVariable("Google_ApiKey");
        }

        /// <summary>
        /// Gets or sets the API key to use.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Looks up the geolocation information associated with the specified latitude and longitude as an asynchronous operation.
        /// </summary>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the result of looking up the specified latitude and longitude.
        /// </returns>
        public async Task<CoordinateLookup> LookupAsync(double latitude, double longitude)
        {
            string requestUri = string.Format(
                CultureInfo.InvariantCulture,
                "https://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}&key={2}",
                latitude,
                longitude,
                Uri.EscapeDataString(ApiKey ?? string.Empty));

            CoordinateLookup result = null;

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync(requestUri))
                {
                    response.EnsureSuccessStatusCode();

                    var lookup = await response.Content.ReadAsAsync<GeocodeResult>();

                    if (lookup != null && lookup.Results.Count > 0)
                    {
                        var item = lookup.Results.First();

                        result = new CoordinateLookup()
                        {
                            City = GetShortName(item, "sublocality_level_1"),
                            CountryCode = GetShortName(item, "country"),
                            PostalCode = GetShortName(item, "postal_code"),
                            RegionCode = GetShortName(item, "administrative_area_level_1"),
                        };

                        if (string.IsNullOrEmpty(result.City))
                        {
                            result.City = GetShortName(item, "postal_town");
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the short name of the address component with the specified name, if found.
        /// </summary>
        /// <param name="value">The value to get the component's short name from.</param>
        /// <param name="componentName">The name of the component to get the short name of.</param>
        /// <returns>
        /// A <see cref="string"/> containing the short name of the specified component, if found.
        /// </returns>
        private static string GetShortName(GeocodeResultItem value, string componentName)
        {
            return value.AddressComponents
                .Where((p) => p.Types.Contains(componentName))
                .Select((p) => p.ShortName)
                .DefaultIfEmpty(string.Empty)
                .FirstOrDefault();
        }

        private sealed class GeocodeResult
        {
            [JsonProperty("results")]
            public ICollection<GeocodeResultItem> Results { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }
        }

        private sealed class GeocodeResultItem
        {
            [JsonProperty("address_components")]
            public ICollection<AddressComponent> AddressComponents { get; set; }

            [JsonProperty("formatted_address")]
            public string FormattedAddress { get; set; }

            [JsonProperty("place_id")]
            public string PlaceId { get; set; }

            [JsonProperty("types")]
            public ICollection<string> Types { get; set; }
        }

        private sealed class AddressComponent
        {
            [JsonProperty("long_name")]
            public string LongName { get; set; }

            [JsonProperty("short_name")]
            public string ShortName { get; set; }

            [JsonProperty("types")]
            public ICollection<string> Types { get; set; }
        }
    }
}
