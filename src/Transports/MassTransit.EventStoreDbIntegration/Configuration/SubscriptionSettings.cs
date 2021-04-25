using System;
using EventStore.Client;

namespace MassTransit.EventStoreDbIntegration
{
    public interface SubscriptionSettings
    {
        StreamName StreamName { get; }
        string SubscriptionName { get; }

        ushort CheckpointMessageCount { get; }
        TimeSpan CheckpointInterval { get; }
        int ConcurrencyLimit { get; }

        IEventFilter AllStreamEventFilter { get; }
        uint AllStreamCheckpointInterval { get; }

        UserCredentials UserCredentials { get; }

        //EventStoreClientOperationOptions ConfigureOperationOptions { get; }

        bool IsCatchupSubscription { get; }
    }
}
