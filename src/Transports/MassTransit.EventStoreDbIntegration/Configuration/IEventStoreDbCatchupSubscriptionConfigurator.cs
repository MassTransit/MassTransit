using System;
using EventStore.Client;

namespace MassTransit.EventStoreDbIntegration
{
    public interface IEventStoreDbCatchupSubscriptionConfigurator :
        IReceiveEndpointConfigurator
    {
        StreamName StreamName { get; }

        string SubscriptionName { get; }

        /// <summary>
        /// Sets interval before checkpoint, low interval will decrease throughput (default: 1min).
        /// </summary>
        TimeSpan CheckpointInterval { set; }

        /// <summary>
        /// Set max message count for checkpoint, low message count will decrease throughput (default: 5000).
        /// </summary>
        ushort CheckpointMessageCount { set; }

        /// <summary>
        /// Event filter when subscribing to the all stream. If not set, will default to ExcludeSystemEvents.
        /// </summary>
        IEventFilter AllStreamEventFilter { set; }

        /// <summary>
        /// User credentials to use for the subscription.
        /// </summary>
        UserCredentials UserCredentials { set; }

        void SetCheckpointStore(CheckpointStoreFactory checkpointStoreFactory);
    }
}
