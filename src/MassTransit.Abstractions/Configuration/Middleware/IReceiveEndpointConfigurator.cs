namespace MassTransit
{
    using System;
    using System.ComponentModel;
    using System.Net.Mime;


    /// <summary>
    /// Configure a receiving endpoint
    /// </summary>
    public interface IReceiveEndpointConfigurator :
        IEndpointConfigurator,
        IReceiveEndpointObserverConnector
    {
        /// <summary>
        /// Returns the input address of the receive endpoint
        /// </summary>
        Uri InputAddress { get; }

        /// <summary>
        /// If true (the default), the broker topology is configured using the message types consumed by
        /// handlers, consumers, sagas, and activities. The implementation is broker-specific, but generally
        /// supported enough to be implemented across the board. This method obsoletes the previous methods,
        /// such as BindMessageTopics, BindMessageExchanges, SubscribeMessageTopics, etc.
        /// </summary>
        bool ConfigureConsumeTopology { set; }

        /// <summary>
        /// If true (the default), faults should be published when no ResponseAddress or FaultAddress are present.
        /// </summary>
        bool PublishFaults { set; }

        /// <summary>
        /// Specify the number of messages to prefetch from the message broker
        /// </summary>
        /// <value>The limit</value>
        int PrefetchCount { get; set; }

        /// <summary>
        /// Specify the number of concurrent messages that can be consumed (separate from prefetch count)
        /// </summary>
        int? ConcurrentMessageLimit { get; set; }

        /// <summary>
        /// When deserializing a message, if no ContentType is present on the receive context, use this as the default
        /// </summary>
        ContentType DefaultContentType { set; }

        /// <summary>
        /// When serializing a message, use the content type specified for serialization
        /// </summary>
        ContentType SerializerContentType { set; }

        /// <summary>
        /// Configures whether the broker topology is configured for the specified message type. Related to
        /// <see cref="ConfigureConsumeTopology" />, but for an individual message type.
        /// </summary>
        void ConfigureMessageTopology<T>(bool enabled = true)
            where T : class;

        [EditorBrowsable(EditorBrowsableState.Never)]
        void AddEndpointSpecification(IReceiveEndpointSpecification configurator);

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

        /// <summary>
        /// Add the observable receive endpoint as a dependency
        /// </summary>
        /// <param name="connector"></param>
        void AddDependency(IReceiveEndpointObserverConnector connector);
    }
}
