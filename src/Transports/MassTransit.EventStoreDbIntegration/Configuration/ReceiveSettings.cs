using System;
using MassTransit.EventStoreDbIntegration.Serializers;

namespace MassTransit.EventStoreDbIntegration
{
    public interface ReceiveSettings
    {
        StreamCategory StreamCategory { get; }
        string SubscriptionName { get; }
        ushort CheckpointMessageCount { get; }
        TimeSpan CheckpointInterval { get; }        
        int ConcurrencyLimit { get; }
    }
}
