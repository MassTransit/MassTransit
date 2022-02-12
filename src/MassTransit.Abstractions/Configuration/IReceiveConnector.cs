namespace MassTransit
{
    using System;


    public interface IReceiveConnector<out TEndpointConfigurator> :
        IReceiveConnector
        where TEndpointConfigurator : IReceiveEndpointConfigurator
    {
        /// <summary>
        /// Adds a receive endpoint
        /// </summary>
        /// <param name="definition">
        /// An endpoint definition, which abstracts specific endpoint behaviors from the transport
        /// </param>
        /// <param name="endpointNameFormatter"></param>
        /// <param name="configureEndpoint">The configuration callback</param>
        HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter = null,
            Action<TEndpointConfigurator>? configureEndpoint = null);

        /// <summary>
        /// Adds a receive endpoint
        /// </summary>
        /// <param name="queueName">The queue name for the receive endpoint</param>
        /// <param name="configureEndpoint">The configuration callback</param>
        HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<TEndpointConfigurator>? configureEndpoint = null);
    }


    public interface IReceiveConnector :
        IEndpointConfigurationObserverConnector
    {
        /// <summary>
        /// Adds a receive endpoint
        /// </summary>
        /// <param name="definition">
        /// An endpoint definition, which abstracts specific endpoint behaviors from the transport
        /// </param>
        /// <param name="endpointNameFormatter"></param>
        /// <param name="configureEndpoint">The configuration callback</param>
        HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter = null,
            Action<IReceiveEndpointConfigurator>? configureEndpoint = null);

        /// <summary>
        /// Adds a receive endpoint
        /// </summary>
        /// <param name="queueName">The queue name for the receive endpoint</param>
        /// <param name="configureEndpoint">The configuration callback</param>
        HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator>? configureEndpoint = null);
    }
}
