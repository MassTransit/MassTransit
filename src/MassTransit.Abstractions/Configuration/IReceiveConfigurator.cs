namespace MassTransit
{
    using System;


    public interface IReceiveConfigurator<out TEndpointConfigurator> :
        IReceiveConfigurator
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
        void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter = null,
            Action<TEndpointConfigurator>? configureEndpoint = null);

        /// <summary>
        /// Adds a receive endpoint
        /// </summary>
        /// <param name="queueName">The queue name for the receive endpoint</param>
        /// <param name="configureEndpoint">The configuration callback</param>
        void ReceiveEndpoint(string queueName, Action<TEndpointConfigurator> configureEndpoint);
    }


    public interface IReceiveConfigurator :
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
        void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter? endpointNameFormatter = null,
            Action<IReceiveEndpointConfigurator>? configureEndpoint = null);

        /// <summary>
        /// Adds a receive endpoint
        /// </summary>
        /// <param name="queueName">The queue name for the receive endpoint</param>
        /// <param name="configureEndpoint">The configuration callback</param>
        void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint);
    }
}
