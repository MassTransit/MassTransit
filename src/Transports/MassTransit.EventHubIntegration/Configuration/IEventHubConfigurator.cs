namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;


    public interface IEventHubConfigurator :
        IReceiveEndpointConfigurator
    {
        /// <summary>
        /// The name of the container in the storage account to reference.
        /// </summary>
        string ContainerName { set; }

        /// <summary>
        /// Sets interval before checkpoint, low interval will decrease throughput (default: 1min)
        /// </summary>
        TimeSpan CheckpointInterval { set; }

        /// <summary>
        /// Set max message count for checkpoint, low message count will decrease throughput (default: 1000)
        /// </summary>
        ushort CheckpointMessageCount { set; }

        /// <summary>
        /// Configure <see cref="EventProcessorClientOptions" />
        /// </summary>
        Action<EventProcessorClientOptions> ConfigureOptions { set; }

        /// <summary>
        /// The event to be raised once event processing stops for a given partition.
        /// </summary>
        void OnPartitionClosing(Func<PartitionClosingEventArgs, Task> handler);

        /// <summary>
        /// The event to be raised just before event processing starts for a given partition.
        /// </summary>
        void OnPartitionInitializing(Func<PartitionInitializingEventArgs, Task> handler);
    }
}
