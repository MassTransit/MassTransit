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
        IConsumePipeConfigurator,
        ISendPipelineConfigurator,
        IPublishPipelineConfigurator,
        IReceiveEndpointObserverConnector
    {
        /// <summary>
        /// Returns the input address of the receive endpoint
        /// </summary>
        Uri InputAddress { get; }

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
