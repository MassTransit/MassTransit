namespace MassTransit.KafkaIntegration
{
    using System;


    public interface ReceiveSettings
    {
        string Topic { get; }
        ushort MessageLimit { get; }
        ushort CheckpointMessageCount { get; }
        int ConcurrencyLimit { get; }
        TimeSpan CheckpointInterval { get; }
    }
}
