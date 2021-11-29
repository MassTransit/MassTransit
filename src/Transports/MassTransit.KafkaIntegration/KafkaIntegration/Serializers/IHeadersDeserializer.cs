namespace MassTransit.KafkaIntegration.Serializers
{
    using Confluent.Kafka;
    using Transports;


    public interface IHeadersDeserializer
    {
        IHeaderProvider Deserialize(Headers headers);
    }
}
