namespace MassTransit
{
    using System;
    using EndpointConfigurators;


    public interface IReceiveConnector<out TEndpointConfigurator> :
        IReceiveConnector
        where TEndpointConfigurator : IReceiveEndpointConfigurator
    {
        /// <summary>
        /// Adds a receive endpoint
        /// </summary>
        /// <param name="definition">An endpoint definition, which abstracts specific endpoint behaviors from the transport</param>
        /// <param name="endpointNameFormatter"></param>
        /// <param name="configureEndpoint">The configuration callback</param>
        HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<TEndpointConfigurator> configureEndpoint = null);

        /// <summary>
        /// Adds a receive endpoint
        /// </summary>
        /// <param name="queueName">The queue name for the receive endpoint</param>
        /// <param name="configureEndpoint">The configuration callback</param>
        HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<TEndpointConfigurator> configureEndpoint);
    }


    public interface IReceiveConnector :
        IEndpointConfigurationObserverConnector
    {
        /// <summary>
        /// Adds a receive endpoint
        /// </summary>
        /// <param name="definition">An endpoint definition, which abstracts specific endpoint behaviors from the transport</param>
        /// <param name="endpointNameFormatter"></param>
        /// <param name="configureEndpoint">The configuration callback</param>
        HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null);

        /// <summary>
        /// Adds a receive endpoint
        /// </summary>
        /// <param name="queueName">The queue name for the receive endpoint</param>
        /// <param name="configureEndpoint">The configuration callback</param>
        HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint);
    }
}
