namespace MassTransit.TestFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using Introspection;
    using Serialization;


    public static class IntrospectionExtensions
    {
        public static IEnumerable<Uri> GetReceiveEndpointAddresses(this IBus bus)
        {
            var probeResult = bus.GetProbeResult();

            var element = JsonSerializer.SerializeToElement(probeResult, SystemTextJsonMessageSerializer.Options);

            Uri[] endpoints = element
                .GetProperty("results")
                .GetProperty("bus")
                .GetProperty("host")
                .GetProperty("receiveEndpoint")
                .EnumerateArray()
                .Select(x => x.GetProperty("receiveTransport").TryGetProperty("address", out var property) ? property.GetString() : null)
                .Where(x => x != null)
                .Select(x => new Uri(x))
                .ToArray();

            return endpoints;
        }

        public static string ToJsonString(this ProbeResult result)
        {
            return JsonSerializer.Serialize(result, typeof(ProbeResult), SystemTextJsonMessageSerializer.Options);
        }
    }
}
