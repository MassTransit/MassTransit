namespace MassTransit.KafkaIntegration
{
    using System;


    public interface KafkaBaseConsumeContext
    {
        string Topic { get; }
        int Partition { get; }
        long Offset { get; }
        DateTime CheckpointUtcDateTime { get; }
    }


    public interface KafkaBaseConsumeContext<out TKey> :
        KafkaBaseConsumeContext
    {
        TKey Key { get; }
    }
}
