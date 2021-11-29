namespace MassTransit
{
    using System;
    using ActiveMqTransport;


    /// <summary>
    /// Configure a receiving ActiveMQ endpoint
    /// </summary>
    public interface IActiveMqReceiveEndpointConfigurator :
        IReceiveEndpointConfigurator,
        IActiveMqQueueEndpointConfigurator
    {
        /// <summary>
        /// Bind an existing exchange for the message type to the receive endpoint by name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Bind<T>(Action<IActiveMqTopicBindingConfigurator> callback = null)
            where T : class;

        /// <summary>
        /// Bind an exchange to the receive endpoint exchange
        /// </summary>
        /// <param name="topicName">The exchange name</param>
        /// <param name="callback">Configure the exchange and binding</param>
        void Bind(string topicName, Action<IActiveMqTopicBindingConfigurator> callback = null);

        void ConfigureSession(Action<IPipeConfigurator<SessionContext>> configure);
        void ConfigureConnection(Action<IPipeConfigurator<ConnectionContext>> configure);
    }
}
