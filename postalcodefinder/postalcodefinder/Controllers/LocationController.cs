using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using Newtonsoft.Json;
using postalcodefinder.Models;

namespace postalcodefinder.Controllers
{
    public class LocationController : ApiController
    {
        // POST api/values
        public IHttpActionResult Post([FromBody][Required]LocationRequest value)
        {
            if (value == null)
            {
                return BadRequest("No request body specified.");
            }

            if (value.Coordinates == null && value.Timestamp == null)
            {
                return BadRequest("No time or coordinates specified.");
            }

            string derivationMethod;

            if (value.Coordinates != null)
            {
                derivationMethod = "coordinates";
            }
            else
            {
                derivationMethod = "timestamp";
            }

            // TODO Actually implement something
            LocationReponse response = new LocationReponse()
            {
                DerivationMethod = derivationMethod,
                Country = "GB",
                PostalCode = "SW11",
            };

            return Ok(response);
        }
    }
}
