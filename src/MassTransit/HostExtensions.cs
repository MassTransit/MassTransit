namespace MassTransit
{
    using System;
    using System.Collections.Generic;


    public static class HostExtensions
    {
        public static void AddReceiveEndpoint(this IHost host, IDictionary<string, IReceiveEndpointControl> endpoints)
        {
            endpoints = endpoints ?? throw new ArgumentNullException(nameof(endpoints));

            foreach (KeyValuePair<string, IReceiveEndpointControl> endpoint in endpoints)
                host.AddReceiveEndpoint(endpoint.Key, endpoint.Value);
        }
    }
}
