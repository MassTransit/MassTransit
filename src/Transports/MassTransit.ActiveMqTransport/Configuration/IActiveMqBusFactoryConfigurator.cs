using MassTransit.ActiveMqTransport.Configurators;

namespace MassTransit.ActiveMqTransport
{
    using System;
    using Configuration;
    using Topology;


    public interface IActiveMqBusFactoryConfigurator :
        IBusFactoryConfigurator<IActiveMqReceiveEndpointConfigurator>,
        IQueueEndpointConfigurator
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
        /// New configurable extensionpoint, available/accessble via the configurator.
        /// This extension point can be used to enable artimis compatibility
        /// It allows to override the generated BindingConsumeTopologySpecification
        /// That specification is responsible (on calling its Apply method) for interacting with an instance of IReceiveEndpointBrokerTopologyBuilder.
        /// This allows the specification to control what queues, bindings, are created.
        /// For the interop with Artemis the name of the consumer queue is important
        /// The original specification was : ActiveMqBindConsumeTopologySpecification
        /// a new one has been already provided for Artemis => ArtemisBindConsumeTopologySpecification
        ///
        /// When you call EnableArtemisCompatibility() => this factory method is automatically initialized with a factory method that will
        /// create a ArtemisBindConsumeTopologySpecification
        ///
        /// This extension could also be used to create your own specifications if you prefer other behavior for creating queues and bindings or
        /// name conventions
        /// </summary>
        public ActiveMqBindingConsumeTopologySpecificationFactoryMethod BindingConsumeTopologySpecificationFactoryMethod
        {
            get;
            set;
        }

        /// <summary>
        /// This is a handy shortcut method that will initialize the required extensionpoints
        /// in order to have the activemq transport working with Artemis
        /// </summary>
        public void EnableArtemisCompatibility();

        void UpdateReceiveQueueName(Func<string, string> transformReceiveQueueName);
    }
}
