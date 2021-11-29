namespace MassTransit
{
    using System;
    using System.Net.Mime;
    using Configuration;


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

        /// <summary>
        /// When deserializing a message, if no ContentType is present on the receive context, use this as the default
        /// </summary>
        ContentType DefaultContentType { set; }

        /// <summary>
        /// When serializing a message, use the content type specified for serialization
        /// </summary>
        ContentType SerializerContentType { set; }

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
        /// Add a message serializer using the specified factory (can be shared by serializer/deserializer)
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="isSerializer">If true, set the current serializer to the specified factory</param>
        void AddSerializer(ISerializerFactory factory, bool isSerializer = true);

        /// <summary>
        /// Add a message deserializer using the specified factory (can be shared by serializer/deserializer)
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="isDefault">If true, set the default content type to the content type of the deserializer</param>
        void AddDeserializer(ISerializerFactory factory, bool isDefault = false);

        /// <summary>
        /// Clears all message serialization configuration
        /// </summary>
        void ClearSerialization();
    }
}
