namespace MassTransit.Transports
{
    using System;
    using System.Linq;


    public static class EndpointAddressExtensions
    {
        /// <summary>
        /// Returns the endpoint name (the last part of the URI, without the query string or preceding path)
        /// from the address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static string? GetEndpointName(this Uri? address)
        {
            return address?.AbsolutePath?.Split('/').LastOrDefault();
        }

        public static string GetDiagnosticEndpointName(this Uri address)
        {
            var endpointName = address.GetEndpointName();
            if (string.IsNullOrWhiteSpace(endpointName))
                return "";

            return endpointName!.Contains("_bus_") ? "bus" : endpointName;
        }
    }
}
