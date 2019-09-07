namespace MassTransit
{
    using System;
    using Transports.InMemory;
    using Transports.InMemory.Topology.Configurators;


    public interface IInMemoryBusFactoryConfigurator :
        IBusFactoryConfigurator<IInMemoryReceiveEndpointConfigurator>,
        IReceiveConfigurator<IInMemoryHost, IInMemoryReceiveEndpointConfigurator>
    {
        /// <summary>
        /// Sets the maximum number of threads used by an in-memory transport, for partitioning
        /// the input queue. This setting also specifies how many threads will be used for dispatching
        /// messages to consumers.
        /// </summary>
        int TransportConcurrencyLimit { set; }

        IInMemoryHost Host { get; }

        new IInMemoryPublishTopologyConfigurator PublishTopology { get; }

        /// <summary>
        /// Configure the send topology of the message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configureTopology"></param>
        void Publish<T>(Action<IInMemoryMessagePublishTopologyConfigurator<T>> configureTopology)
            where T : class;
    }
}
