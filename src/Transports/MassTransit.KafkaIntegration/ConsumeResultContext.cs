namespace MassTransit.KafkaIntegration
{
    using Confluent.Kafka;


    public interface ConsumeResultContext
    {
        string Topic { get; }
        Partition Partition { get; }
        Offset Offset { get; }
        Timestamp Timestamp { get; }
        Headers Headers { get; }
    }
}
