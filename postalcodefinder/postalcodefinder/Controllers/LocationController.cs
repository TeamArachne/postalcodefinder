namespace postalcodefinder.Controllers
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using FreeGeoIP.Client;
    using Models;
    using Newtonsoft.Json;
    using VisitorIQ.Client;

    public class LocationController : ApiController
    {
        [EnableCors(origins: "*", headers: "*", methods: "POST")]
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

                response = await LookupByCoordinatesAsync(value.Coordinates);
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

        private static async Task<LocationReponse> LookupByCoordinatesAsync(LocationRequest.LocationCoordinates coordinates)
        {
            string country = string.Empty;
            string region = string.Empty;
            string postalCode = string.Empty;
            string city = string.Empty;

            var connectionString = ConfigurationManager.ConnectionStrings["SqlAzure"];

            if (connectionString != null && !string.IsNullOrEmpty(connectionString.ConnectionString))
            {
                using (var connection = new SqlConnection(connectionString.ConnectionString))
                {
                    await connection.OpenAsync();

                    try
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Parameters.Add(new SqlParameter("@lat", coordinates.Latitude));
                            command.Parameters.Add(new SqlParameter("@long", coordinates.Longitude));

                            command.CommandText = @"
declare @g geography = geography::Point(@lat, @long, 4326)

select top 1 [Iso2],
             [PostalCode],
             [PlaceName],
             [StateCode]
from [dbo].[tblGB]
where [GeoLocation].STDistance(@g) is not null
order by [GeoLocation].STDistance(@g);
";

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    city = Convert.ToString(reader["PlaceName"], CultureInfo.InvariantCulture);
                                    country = Convert.ToString(reader["Iso2"], CultureInfo.InvariantCulture);
                                    postalCode = Convert.ToString(reader["PostalCode"], CultureInfo.InvariantCulture);
                                    region = Convert.ToString(reader["StateCode"], CultureInfo.InvariantCulture);

                                    break;
                                }
                            }
                        }
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            else
            {
                var client = new Google.Geocoding.Client.GeocodingClient()
                {
                    ApiKey = ConfigurationManager.AppSettings["Google_ApiKey"],
                };

                var lookup = await client.LookupAsync(coordinates.Latitude, coordinates.Longitude);

                if (lookup != null)
                {
                    city = lookup.City;
                    country = lookup.CountryCode;
                    region = lookup.RegionCode;
                    postalCode = lookup.PostalCode;
                }
            }

            return new LocationReponse()
            {
                DerivationMethod = "coordinates",
                City = city,
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
                City = string.Empty,
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
                        City = location.City,
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
                        City = location.City,
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
            string city = string.Empty;

            if (value.Offset == TimeSpan.FromHours(-5))
            {
                country = "US";
                region = "NY";
                postalCode = "10547";
                city = "Mohegan Lake";
            }
            else if (value.Offset == TimeSpan.FromHours(-6))
            {
                country = "US";
                region = "TX";
                postalCode = "75155";
                city = "Rice";
            }
            else if (value.Offset == TimeSpan.FromHours(-7))
            {
                country = "US";
                region = "CO";
                postalCode = "80208";
                city = "Denver";
            }
            else if (value.Offset == TimeSpan.FromHours(-8))
            {
                country = "US";
                region = "WA";
                postalCode = "98009";
                city = "Bellevue";
            }

            return new LocationReponse()
            {
                DerivationMethod = "timestamp",
                City = city,
                Country = country,
                PostalCode = postalCode,
                Region = region,
            };
        }
    }
}
