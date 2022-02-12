namespace MassTransit
{
    using System;


    public static class HostConnectReceiveEndpointExtensions
    {
        /// <summary>
        /// Connect a response endpoint for the host
        /// </summary>
        /// <param name="connector">The host to connect</param>
        /// <param name="endpointNameFormatter"></param>
        /// <param name="configureEndpoint">The configuration callback</param>
        public static HostReceiveEndpointHandle ConnectResponseEndpoint(this IReceiveConnector connector,
            IEndpointNameFormatter? endpointNameFormatter = null,
            Action<IReceiveEndpointConfigurator>? configureEndpoint = null)
        {
            return connector.ConnectReceiveEndpoint(new ResponseEndpointDefinition(), endpointNameFormatter, configureEndpoint);
        }

        /// <summary>
        /// Connect an endpoint for the host
        /// </summary>
        /// <param name="connector">The host to connect</param>
        /// <param name="configureEndpoint">The configuration callback</param>
        public static HostReceiveEndpointHandle ConnectReceiveEndpoint(this IReceiveConnector connector,
            Action<IReceiveEndpointConfigurator>? configureEndpoint = null)
        {
            return connector.ConnectReceiveEndpoint(new TemporaryEndpointDefinition(), null, configureEndpoint);
        }
    }
}
