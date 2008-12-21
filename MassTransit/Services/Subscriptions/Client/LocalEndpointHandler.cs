namespace MassTransit.Services.Subscriptions.Client
{
    using System;
    using System.Collections.Generic;

    public class LocalEndpointHandler
    {
        private readonly List<Uri> _localEndpoints = new List<Uri>();

        public void AddLocalEndpoint(IEndpoint endpoint)
        {
            _localEndpoints.Add(endpoint.Uri);
        }

        public bool ContainsEndpoint(IEndpoint endpoint)
        {
            return _localEndpoints.Contains(endpoint.Uri);
        }
        public bool ContainsEndpoint(Uri endpoint)
        {
            return _localEndpoints.Contains(endpoint);
        }
    }
}