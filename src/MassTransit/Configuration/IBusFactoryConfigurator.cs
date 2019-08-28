namespace MassTransit
{
    using System;
    using System.ComponentModel;
    using System.Net.Mime;
    using Builders;
    using GreenPipes;
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
        IPublishPipelineConfigurator
    {
        /// <summary>
        /// Connects a bus observer to the bus to observe lifecycle events on the bus
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        ConnectHandle ConnectBusObserver(IBusObserver observer);

        IMessageTopologyConfigurator MessageTopology { get; }

        ISendTopologyConfigurator SendTopology { get; }

        IPublishTopologyConfigurator PublishTopology { get; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        void AddBusFactorySpecification(IBusFactorySpecification specification);

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
