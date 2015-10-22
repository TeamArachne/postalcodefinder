using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace postalcodefinder.Models
{
    public class LocationRequest
    {
        [JsonProperty("datetime")]
        public DateTimeOffset? Timestamp { get; set; }

        [JsonProperty("coords")]
        public LocationCoordinates Coordinates { get; set; }

        public class LocationCoordinates
        {
            [JsonProperty("lat")]
            [Required]
            public double Latitude { get; set; }

            [JsonProperty("long")]
            [Required]
            public double Longitude { get; set; }
        }
    }
}
