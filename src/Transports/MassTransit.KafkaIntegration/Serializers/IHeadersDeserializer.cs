namespace MassTransit.KafkaIntegration.Serializers
{
    using Confluent.Kafka;
    using Context;


    public interface IHeadersDeserializer
    {
        IHeaderProvider Deserialize(Headers headers);
    }
}
