#nullable enable
namespace MassTransit
{
    using System;
    using ActiveMqTransport;
    using ActiveMqTransport.Topology;


    public interface IActiveMqBusFactoryConfigurator :
        IBusFactoryConfigurator<IActiveMqReceiveEndpointConfigurator>,
        IActiveMqQueueEndpointConfigurator
    {
        new IActiveMqSendTopologyConfigurator SendTopology { get; }

        new IActiveMqPublishTopologyConfigurator PublishTopology { get; }

        /// <summary>
        /// Configure the send topology of the message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configureTopology"></param>
        void Send<T>(Action<IActiveMqMessageSendTopologyConfigurator<T>> configureTopology)
            where T : class;

        /// <summary>
        /// Configure the send topology of the message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configureTopology"></param>
        void Publish<T>(Action<IActiveMqMessagePublishTopologyConfigurator<T>> configureTopology)
            where T : class;

        /// <summary>
        /// Configure a Host that can be connected. If only one host is specified, it is used as the default
        /// host for receive endpoints.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        void Host(ActiveMqHostSettings settings);

        /// <summary>
        /// Configure the consumer topology so that it is compatible for the format required for ActiveMQ Artemis
        /// </summary>
        void EnableArtemisCompatibility();

        /// <summary>
        /// Specify the prefix to be added to temporary queue names, including the bus endpoint
        /// </summary>
        /// <param name="prefix"></param>
        void SetTemporaryQueueNamePrefix(string prefix);

        /// <summary>
        /// Specify a consumer endpoint queue name formatter. Generate name for consumer queue using
        /// topic and endpoint name
        /// </summary>
        /// <param name="formatter"></param>
        public void SetConsumerEndpointQueueNameFormatter(IActiveMqConsumerEndpointQueueNameFormatter formatter);

        /// <summary>
        /// Specify a temporary queue name formatter. Allows for the transformation of masstransit generated temporary queuenames
        /// e.g. adding a prefix
        /// </summary>
        /// <param name="formatter"></param>
        public void SetTemporaryQueueNameFormatter(IActiveMqTemporaryQueueNameFormatter? formatter);
    }
}
