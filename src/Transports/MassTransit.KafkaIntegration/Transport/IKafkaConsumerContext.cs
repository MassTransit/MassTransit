namespace MassTransit.KafkaIntegration.Transport
{
    using Confluent.Kafka;
    using GreenPipes;
    using Serializers;


    public interface IKafkaConsumerContext<TKey, TValue> :
        PipeContext
        where TValue : class
    {
        ReceiveSettings ReceiveSettings { get; }
        IHeadersDeserializer HeadersDeserializer { get; }

        ConsumerBuilder<TKey, TValue> CreateConsumerBuilder();
    }
}
