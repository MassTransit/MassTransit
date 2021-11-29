namespace MassTransit
{
    using System;


    public interface KafkaConsumeContext
    {
        string Topic { get; }
        int Partition { get; }
        long Offset { get; }
        DateTime CheckpointUtcDateTime { get; }
    }


    public interface KafkaConsumeContext<out TKey> :
        KafkaConsumeContext
    {
        TKey Key { get; }
    }
}
