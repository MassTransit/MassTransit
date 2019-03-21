namespace MassTransit.Conductor
{
    using System;


    public interface IServiceInstanceConfigurator<out TEndpointConfigurator>
        where TEndpointConfigurator : IReceiveEndpointConfigurator
    {
        /// <summary>
        /// Specify a receive endpoint for the bus, using an endpoint definition
        /// </summary>
        /// <param name="definition">An endpoint definition, which abstracts specific endpoint behaviors from the transport</param>
        /// <param name="endpointNameFormatter"></param>
        /// <param name="configureEndpoint">The configuration callback</param>
        void ReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter = null,
            Action<TEndpointConfigurator> configureEndpoint = null);

        /// <summary>
        /// Specify a receive endpoint for the bus, with the specified queue name
        /// </summary>
        /// <param name="queueName">The queue name for the receiving endpoint</param>
        /// <param name="configureEndpoint">The configuration callback</param>
        void ReceiveEndpoint(string queueName, Action<TEndpointConfigurator> configureEndpoint);
    }
}