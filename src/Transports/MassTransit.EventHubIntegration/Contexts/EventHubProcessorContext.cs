namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Azure.Storage.Blobs;
    using GreenPipes;


    public class EventHubProcessorContext :
        BasePipeContext,
        IEventHubProcessorContext
    {
        public EventHubProcessorContext(ReceiveSettings receiveSettings, BlobContainerClient blobContainerClient, EventProcessorClient client,
            CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            client.PartitionInitializingAsync += OnPartitionInitializing;
            client.PartitionClosingAsync += OnPartitionClosing;

            ReceiveSettings = receiveSettings;
            BlobContainerClient = blobContainerClient;
            EventProcessorClient = client;
        }

        async Task OnPartitionClosing(PartitionClosingEventArgs arg)
        {
            if (PartitionClosing != null)
                await PartitionClosing.Invoke(arg).ConfigureAwait(false);
        }

        async Task OnPartitionInitializing(PartitionInitializingEventArgs arg)
        {
            if (PartitionInitializing != null)
                await PartitionInitializing.Invoke(arg).ConfigureAwait(false);
        }

        public event Func<PartitionInitializingEventArgs, Task> PartitionInitializing;
        public event Func<PartitionClosingEventArgs, Task> PartitionClosing;

        public BlobContainerClient BlobContainerClient { get; }

        public EventProcessorClient EventProcessorClient { get; }

        public ReceiveSettings ReceiveSettings { get; }
    }
}
