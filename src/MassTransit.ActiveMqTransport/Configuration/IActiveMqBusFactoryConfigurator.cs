namespace MassTransit.ActiveMqTransport
{
    using System;
    using Topology;


    public interface IActiveMqBusFactoryConfigurator :
        IBusFactoryConfigurator<IActiveMqReceiveEndpointConfigurator>,
        IReceiveConfigurator<IActiveMqHost, IActiveMqReceiveEndpointConfigurator>,
        IQueueEndpointConfigurator
    {
        new IActiveMqSendTopologyConfigurator SendTopology { get; }

        new IActiveMqPublishTopologyConfigurator PublishTopology { get; }

        /// <summary>
        /// Set to true if the topology should be deployed only
        /// </summary>
        bool DeployTopologyOnly { set; }

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
        IActiveMqHost Host(ActiveMqHostSettings settings);
    }
}
