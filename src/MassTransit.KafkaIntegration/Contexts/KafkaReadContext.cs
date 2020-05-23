namespace MassTransit.KafkaIntegration.Contexts
{
    using Confluent.Kafka;


    public interface KafkaReadContext
    {
        string Topic { get; }
        Partition Partition { get; }
        Offset Offset { get; }
        Timestamp Timestamp { get; }
        Headers Headers { get; }
    }
}
