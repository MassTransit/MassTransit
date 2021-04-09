using System;
using EventStore.Client;

namespace MassTransit.EventStoreDbIntegration
{
    public interface IEventStoreDbReceiveEndpointConfigurator :
        IReceiveEndpointConfigurator
    {
        /// <summary>
        /// Sets interval before checkpoint, low interval will decrease throughput (default: 1min).
        /// </summary>
        TimeSpan CheckpointInterval { set; }

        /// <summary>
        /// Set max message count for checkpoint, low message count will decrease throughput (default: 5000).
        /// </summary>
        ushort CheckpointMessageCount { set; }

        /// <summary>
        /// Filter options when subscribing to the all stream.
        /// </summary>
        Action<SubscriptionFilterOptions> FilterOptions { set; }

        /// <summary>
        /// User credentials used for the subscription.
        /// </summary>
        Action<UserCredentials> UserCredentials { set; }

        void SetCheckpointStore(CheckpointStoreFactory checkpointStoreFactory);
    }
}
