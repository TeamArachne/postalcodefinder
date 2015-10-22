namespace postalcodefinder.Controllers
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;
    using Newtonsoft.Json;
    using postalcodefinder.Models;

    public class LocationController : ApiController
    {
        public IHttpActionResult Post([FromBody][Required]LocationRequest value)
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
                response = LookupByIPAddress(Request);
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

        private static LocationReponse LookupByIPAddress(HttpRequestMessage request)
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

            string country = string.Empty;
            string region = string.Empty;
            string postalCode = string.Empty;

            if (!string.IsNullOrEmpty(clientIPAddress))
            {
                // TODO Lookup based on IP address
            }

            return new LocationReponse()
            {
                DerivationMethod = "ipAddress",
                Country = country,
                PostalCode = postalCode,
                Region = region,
            };
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
