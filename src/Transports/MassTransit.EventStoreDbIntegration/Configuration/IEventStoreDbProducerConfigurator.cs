using MassTransit.EventStoreDbIntegration.Serializers;

namespace MassTransit.EventStoreDbIntegration
{
    public interface IEventStoreDbProducerConfigurator :
        ISendObserverConnector,
        ISendPipelineConfigurator
    {
        /// <summary>
        /// Set the serializer to use to serialize headers.
        /// </summary>
        /// <param name="serializer"></param>
        void SetHeadersSerializer(IHeadersSerializer serializer);

        /// <summary>
        /// Sets the outbound message serializer
        /// </summary>
        /// <param name="serializerFactory">The factory to create the message serializer</param>
        void SetMessageSerializer(SerializerFactory serializerFactory);
    }
}
