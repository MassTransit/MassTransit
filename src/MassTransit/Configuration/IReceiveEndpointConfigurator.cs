namespace MassTransit
{
    using System;
    using System.ComponentModel;
    using System.Net.Mime;
    using Transports;


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

        [EditorBrowsable(EditorBrowsableState.Never)]
        void AddEndpointSpecification(IReceiveEndpointSpecification configurator);

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

        /// <summary>
        /// Add the observable receive endpoint as a dependency
        /// </summary>
        /// <param name="connector"></param>
        void AddDependency(IReceiveEndpointObserverConnector connector);
    }
}
