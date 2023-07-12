namespace MassTransit.KafkaIntegration
{
    using System;


    public interface ReceiveSettings
    {
        long Offset { get; }
        string Topic { get; }
        ushort MessageLimit { get; }
        ushort CheckpointMessageCount { get; }
        int PrefetchCount { get; }
        int ConcurrentDeliveryLimit { get; }
        int ConcurrentMessageLimit { get; }
        ushort ConcurrentConsumerLimit { get; }
        TimeSpan CheckpointInterval { get; }
    }
}
