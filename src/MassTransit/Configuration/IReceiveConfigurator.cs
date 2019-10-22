namespace MassTransit
{
    using System;
    using EndpointConfigurators;


    public interface IReceiveConfigurator<in THost, out TEndpointConfigurator> :
        IEndpointConfigurationObserverConnector
        where THost : IHost
        where TEndpointConfigurator : IReceiveEndpointConfigurator
    {
        /// <summary>
        /// Adds a receive endpoint
        /// </summary>
        /// <param name="host"></param>
        /// <param name="definition">An endpoint definition, which abstracts specific endpoint behaviors from the transport</param>
        /// <param name="endpointNameFormatter"></param>
        /// <param name="configureEndpoint">The configuration callback</param>
        [Obsolete("The host parameter is no longer required, and can be removed")]
        void ReceiveEndpoint(THost host, IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<TEndpointConfigurator> configureEndpoint = null);

        /// <summary>
        /// Adds a receive endpoint
        /// </summary>
        /// <param name="host"></param>
        /// <param name="queueName">The queue name for the receive endpoint</param>
        /// <param name="configureEndpoint">The configuration callback</param>
        [Obsolete("The host parameter is no longer required, and can be removed")]
        void ReceiveEndpoint(THost host, string queueName, Action<TEndpointConfigurator> configureEndpoint);
    }


    public interface IReceiveConfigurator<out TEndpointConfigurator> :
        IReceiveConfigurator
        where TEndpointConfigurator : IReceiveEndpointConfigurator
    {
        /// <summary>
        /// Adds a receive endpoint
        /// </summary>
        /// <param name="definition">An endpoint definition, which abstracts specific endpoint behaviors from the transport</param>
        /// <param name="endpointNameFormatter"></param>
        /// <param name="configureEndpoint">The configuration callback</param>
        void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<TEndpointConfigurator> configureEndpoint = null);

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
        /// <param name="definition">An endpoint definition, which abstracts specific endpoint behaviors from the transport</param>
        /// <param name="endpointNameFormatter"></param>
        /// <param name="configureEndpoint">The configuration callback</param>
        void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IReceiveEndpointConfigurator> configureEndpoint = null);

        /// <summary>
        /// Adds a receive endpoint
        /// </summary>
        /// <param name="queueName">The queue name for the receive endpoint</param>
        /// <param name="configureEndpoint">The configuration callback</param>
        void ReceiveEndpoint(string queueName, Action<IReceiveEndpointConfigurator> configureEndpoint);
    }
}
