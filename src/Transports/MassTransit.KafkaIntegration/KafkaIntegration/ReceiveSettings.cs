namespace MassTransit.KafkaIntegration
{
    using System;


    public interface ReceiveSettings
    {
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
