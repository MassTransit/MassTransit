namespace MassTransit
{
    using System;


    public interface IReceiveEndpointConnector<out TEndpointConfigurator> :
        IReceiveEndpointConnector
        where TEndpointConfigurator : IReceiveEndpointConfigurator
    {
        /// <summary>
        /// Connects a receive endpoint to the bus
        /// </summary>
        /// <param name="definition">
        /// An endpoint definition, which abstracts specific endpoint behaviors from the transport
        /// </param>
        /// <param name="endpointNameFormatter"></param>
        /// <param name="configure">The configuration callback</param>
        HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IBusRegistrationContext, TEndpointConfigurator> configure = null);

        /// <summary>
        /// Connects a receive endpoint to the bus
        /// </summary>
        /// <param name="queueName">The queue name for the receive endpoint</param>
        /// <param name="configure">The configuration callback</param>
        HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IBusRegistrationContext, TEndpointConfigurator> configure = null);
    }


    public interface IReceiveEndpointConnector
    {
        /// <summary>
        /// Connects a receive endpoint to the bus
        /// </summary>
        /// <param name="definition">
        /// An endpoint definition, which abstracts specific endpoint behaviors from the transport
        /// </param>
        /// <param name="endpointNameFormatter"></param>
        /// <param name="configure">The configuration callback</param>
        HostReceiveEndpointHandle ConnectReceiveEndpoint(IEndpointDefinition definition, IEndpointNameFormatter endpointNameFormatter,
            Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure = null);

        /// <summary>
        /// Connects a receive endpoint to the bus
        /// </summary>
        /// <param name="queueName">The queue name for the receive endpoint</param>
        /// <param name="configure">The configuration callback</param>
        HostReceiveEndpointHandle ConnectReceiveEndpoint(string queueName, Action<IBusRegistrationContext, IReceiveEndpointConfigurator> configure = null);
    }
}
