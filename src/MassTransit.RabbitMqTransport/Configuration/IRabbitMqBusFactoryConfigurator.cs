namespace MassTransit.RabbitMqTransport
{
    using System;
    using Topology;


    public interface IRabbitMqBusFactoryConfigurator :
        IBusFactoryConfigurator<IRabbitMqReceiveEndpointConfigurator>,
        IReceiveConfigurator<IRabbitMqHost, IRabbitMqReceiveEndpointConfigurator>,
        IQueueEndpointConfigurator
    {
        new IRabbitMqSendTopologyConfigurator SendTopology { get; }

        new IRabbitMqPublishTopologyConfigurator PublishTopology { get; }

        /// <summary>
        /// Set to true if the topology should be deployed only
        /// </summary>
        bool DeployTopologyOnly { set; }

        /// <summary>
        /// Configure the send topology of the message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configureTopology"></param>
        void Send<T>(Action<IRabbitMqMessageSendTopologyConfigurator<T>> configureTopology)
            where T : class;

        /// <summary>
        /// Configure the send topology of the message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configureTopology"></param>
        void Publish<T>(Action<IRabbitMqMessagePublishTopologyConfigurator<T>> configureTopology)
            where T : class;

        /// <summary>
        /// In most cases, this is not needed and should not be used. However, if for any reason the default bus
        /// endpoint queue name needs to be changed, this will do it. Do NOT set it to the same name as a receive
        /// endpoint or you will screw things up.
        /// </summary>
        void OverrideDefaultBusEndpointQueueName(string queueName);

        /// <summary>
        /// Configure a Host that can be connected. If only one host is specified, it is used as the default
        /// host for receive endpoints.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        IRabbitMqHost Host(RabbitMqHostSettings settings);
    }
}
