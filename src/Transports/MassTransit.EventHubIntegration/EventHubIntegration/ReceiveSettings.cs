namespace MassTransit.EventHubIntegration
{
    using System;


    public interface ReceiveSettings
    {
        string ConsumerGroup { get; }
        string ContainerName { get; }
        string EventHubName { get; }
        ushort CheckpointMessageLimit { get; }
        ushort CheckpointMessageCount { get; }
        int PrefetchCount { get; }
        TimeSpan CheckpointInterval { get; }
        int ConcurrentMessageLimit { get; }
        int ConcurrentDeliveryLimit { get; }
    }
}
