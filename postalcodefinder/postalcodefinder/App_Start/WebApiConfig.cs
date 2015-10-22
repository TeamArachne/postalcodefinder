using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace postalcodefinder
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.EnableCors();

            var formatter = new JsonMediaTypeFormatter();

            // Serialize and deserialize JSON as "myProperty" => "MyProperty" -> "myProperty"
            formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Make JSON easier to read for debugging at the expense of larger payloads
            formatter.SerializerSettings.Formatting = Formatting.Indented;

            // Omit nulls to reduce payload size
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            // Explicitly define behavior when serializing DateTime values
            formatter.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";   // Only return DateTimes to a 1 second precision.
            formatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;       // Only return UTC for DateTimes values.

            // Only support JSON for REST API requests
            config.Formatters.Clear();
            config.Formatters.Add(formatter);

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
