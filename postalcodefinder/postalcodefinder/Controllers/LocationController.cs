namespace postalcodefinder.Controllers
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using FreeGeoIP.Client;
    using Models;
    using Newtonsoft.Json;
    using VisitorIQ.Client;

    public class LocationController : ApiController
    {
        public async Task<IHttpActionResult> Post([FromBody][Required]LocationRequest value)
        {
            if (value == null)
            {
                return BadRequest("No request body specified.");
            }

            LocationReponse response;

            if (value.Coordinates != null)
            {
                if (value.Coordinates.Latitude > 90.0 || value.Coordinates.Longitude < -90.0)
                {
                    return BadRequest("Invalid latitude specified.");
                }
                else if (value.Coordinates.Longitude > 180.0 || value.Coordinates.Longitude < -180.0)
                {
                    return BadRequest("Invalid longitude specified.");
                }

                response = LookupByCoordinates(value.Coordinates);
            }
            else if (value.Timestamp.HasValue)
            {
                response = LookupByTimestamp(value.Timestamp.Value);
            }
            else
            {
                response = await LookupByIPAddressAsync(Request);
            }

            return Ok(response);
        }

        private static LocationReponse LookupByCoordinates(LocationRequest.LocationCoordinates coordinates)
        {
            // TODO Do proper lookup by latitude and longitude
            string country = string.Empty;
            string region = string.Empty;
            string postalCode = string.Empty;

            if (coordinates.Latitude > 48.0 && coordinates.Latitude < 59.0 &&
                coordinates.Longitude > -5.0 && coordinates.Longitude < 2.0)
            {
                country = "GB";
                region = "ENG";
                postalCode = "SW11";
            }

            return new LocationReponse()
            {
                DerivationMethod = "coordinates",
                Country = country,
                PostalCode = postalCode,
                Region = region,
            };
        }

        private static async Task<LocationReponse> LookupByIPAddressAsync(HttpRequestMessage request)
        {
            object property;
            HttpContextBase httpContext;

            string clientIPAddress = null;

            if (request.Properties.TryGetValue("MS_HttpContext", out property) && (httpContext = property as HttpContextBase) != null)
            {
                clientIPAddress = httpContext.Request.UserHostAddress;
            }
            else if (HttpContext.Current != null)
            {
                clientIPAddress = HttpContext.Current.Request.UserHostAddress;
            }

            if (!string.IsNullOrEmpty(clientIPAddress))
            {
                var location = await LookupByIPAddressAsync(clientIPAddress);

                if (location != null)
                {
                    return location;
                }
            }

            return new LocationReponse()
            {
                DerivationMethod = "ipAddress",
                Country = string.Empty,
                PostalCode = string.Empty,
                Region = string.Empty,
            };
        }

        private static async Task<LocationReponse> LookupByIPAddressAsync(string hostNameOrIPAddress)
        {
            LocationReponse result = null;

            if (string.Equals(ConfigurationManager.AppSettings["VisitorIQ_Enabled"], bool.TrueString, StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrEmpty(ConfigurationManager.AppSettings["VisitorIQ_ClientId"]) &&
                !string.IsNullOrEmpty(ConfigurationManager.AppSettings["VisitorIQ_HostName"]))
            {
                var client = new VisitorIQClient()
                {
                    ClientId = ConfigurationManager.AppSettings["VisitorIQ_ClientId"],
                    HostName = ConfigurationManager.AppSettings["VisitorIQ_HostName"],
                };

                var location = await client.LookupAsync(hostNameOrIPAddress);

                if (location != null)
                {
                    return new LocationReponse()
                    {
                        DerivationMethod = "ipAddress",
                        Country = location.CountryCode,
                        PostalCode = location.PostalCode,
                        Region = location.RegionCode,
                    };
                }
            }
            else
            {
                var client = new FreeGeoIPClient();
                var location = await client.LookupAsync(hostNameOrIPAddress);

                if (location != null)
                {
                    return new LocationReponse()
                    {
                        DerivationMethod = "ipAddress",
                        Country = location.CountryCode,
                        PostalCode = location.PostalCode,
                        Region = location.RegionCode,
                    };
                }
            }

            return result;
        }

        private static LocationReponse LookupByTimestamp(DateTimeOffset value)
        {
            string country = "GB";
            string region = "ENG";
            string postalCode = "SW11";

            if (value.Offset == TimeSpan.FromHours(-5))
            {
                country = "US";
                region = "NY";
                postalCode = "10547";
            }
            else if (value.Offset == TimeSpan.FromHours(-6))
            {
                country = "US";
                region = "TX";
                postalCode = "75155";
            }
            else if (value.Offset == TimeSpan.FromHours(-7))
            {
                country = "US";
                region = "CO";
                postalCode = "80208";
            }
            else if (value.Offset == TimeSpan.FromHours(-8))
            {
                country = "US";
                region = "WA";
                postalCode = "98009";
            }

            return new LocationReponse()
            {
                DerivationMethod = "timestamp",
                Country = country,
                PostalCode = postalCode,
                Region = region,
            };
        }
    }
}
