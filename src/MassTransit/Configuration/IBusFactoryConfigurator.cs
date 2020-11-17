namespace MassTransit
{
    using System;
    using System.Net.Mime;
    using BusConfigurators;
    using Topology;


    public interface IBusFactoryConfigurator<out TEndpointConfigurator> :
        IBusFactoryConfigurator,
        IReceiveConfigurator<TEndpointConfigurator>
        where TEndpointConfigurator : IReceiveEndpointConfigurator
    {
    }


    public interface IBusFactoryConfigurator :
        IReceiveConfigurator,
        IConsumePipeConfigurator,
        ISendPipelineConfigurator,
        IPublishPipelineConfigurator,
        IBusObserverConnector,
        IReceiveObserverConnector,
        IConsumeObserverConnector,
        ISendObserverConnector,
        IPublishObserverConnector
    {
        IMessageTopologyConfigurator MessageTopology { get; }
        IConsumeTopologyConfigurator ConsumeTopology { get; }
        ISendTopologyConfigurator SendTopology { get; }
        IPublishTopologyConfigurator PublishTopology { get; }

        /// <summary>
        /// Set to true if the topology should be deployed only
        /// </summary>
        bool DeployTopologyOnly { set; }

        /// <summary>
        /// Specify the number of messages to prefetch from the message broker
        /// </summary>
        /// <value>The limit</value>
        int PrefetchCount { set; }

        /// <summary>
        /// Specify the number of concurrent messages that can be consumed (separate from prefetch count)
        /// </summary>
        int? ConcurrentMessageLimit { set; }
		
		/// Set to true if you want to use the relational db outbox
        /// </summary>
        bool UseOutboxTransport { set; }

        /// <summary>
        /// Configure the message topology for the message type (global across all bus instances of the same transport type)
        /// </summary>
        /// <param name="configureTopology"></param>
        /// <typeparam name="T"></typeparam>
        void Message<T>(Action<IMessageTopologyConfigurator<T>> configureTopology)
            where T : class;

        /// <summary>
        /// Configure the send topology of the message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configureTopology"></param>
        void Send<T>(Action<IMessageSendTopologyConfigurator<T>> configureTopology)
            where T : class;

        /// <summary>
        /// Configure the send topology of the message type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configureTopology"></param>
        void Publish<T>(Action<IMessagePublishTopologyConfigurator<T>> configureTopology)
            where T : class;

        /// <summary>
        /// Sets the outbound message serializer
        /// </summary>
        /// <param name="serializerFactory">The factory to create the message serializer</param>
        void SetMessageSerializer(SerializerFactory serializerFactory);

        /// <summary>
        /// Adds an inbound message deserializer to the available deserializers
        /// </summary>
        /// <param name="contentType">The content type of the deserializer</param>
        /// <param name="deserializerFactory">The factory to create the deserializer</param>
        void AddMessageDeserializer(ContentType contentType, DeserializerFactory deserializerFactory);

        /// <summary>
        /// Clears all message deserializers
        /// </summary>
        void ClearMessageDeserializers();
    }
}
