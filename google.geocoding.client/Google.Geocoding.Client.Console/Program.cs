namespace Google.Geocoding.Client.Console
{
    using System;
    using System.Globalization;

    /// <summary>
    /// An application that looks up geolocation data by latitude and longitude. This class cannot be inherited.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry-point to the application.
        /// </summary>
        /// <param name="args">The arguments passed to the application.</param>
        internal static void Main(string[] args)
        {
            var client = new GeocodingClient();

            if (args != null && args.Length == 1)
            {
                client.ApiKey = args[0];
            }

            string coordinates = null;

            while (true)
            {
                Console.Write("Coordinates: ");
                coordinates = Console.ReadLine();

                if (string.IsNullOrEmpty(coordinates))
                {
                    break;
                }

                string[] parts = coordinates.Split(',');

                if (parts.Length != 2)
                {
                    continue;
                }

                double latitude;
                double longitude;

                if (!double.TryParse(parts[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out latitude))
                {
                    continue;
                }

                if (!double.TryParse(parts[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out longitude))
                {
                    continue;
                }

                CoordinateLookup result;

                try
                {
                    result = client.LookupAsync(latitude, longitude).Result;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine();
                    Console.Error.WriteLine("Failed to look up coordinates: {0}", ex.ToString());
                    continue;
                }

                Console.WriteLine();
                Console.WriteLine("Result for {0}:", coordinates);
                Console.WriteLine();

                if (result == null)
                {
                    Console.WriteLine("  Not found.");
                }
                else
                {
                    Console.WriteLine("     Lat/Long: {0:N2},{1:N2}", latitude, longitude);
                    Console.WriteLine("         City: {0}", result.City);
                    Console.WriteLine("  Postal Code: {0}", result.PostalCode);
                    Console.WriteLine("       Region: {0}", result.RegionCode);
                    Console.WriteLine("      Country: {0}", result.CountryCode);
                }

                Console.WriteLine();
            }
        }
    }
}
