namespace MassTransit
{
    using System;


    public interface KafkaConsumeContext
    {
        string GroupId { get; }
        string Topic { get; }
        int Partition { get; }
        long Offset { get; }
        bool IsPartitionEof { get; }
        DateTime CheckpointUtcDateTime { get; }
    }


    public interface KafkaConsumeContext<out TKey> :
        KafkaConsumeContext
    {
        TKey Key { get; }
    }
}
