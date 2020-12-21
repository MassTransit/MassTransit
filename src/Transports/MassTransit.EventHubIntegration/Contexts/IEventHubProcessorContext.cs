namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Azure.Storage.Blobs;
    using GreenPipes;


    public interface IEventHubProcessorContext :
        PipeContext
    {
        event Func<PartitionInitializingEventArgs, Task> PartitionInitializing;
        event Func<PartitionClosingEventArgs, Task> PartitionClosing;

        BlobContainerClient BlobContainerClient { get; }
        EventProcessorClient EventProcessorClient { get; }

        ReceiveSettings ReceiveSettings { get; }
    }
}
