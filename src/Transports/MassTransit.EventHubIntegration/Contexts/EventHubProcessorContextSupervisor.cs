namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Azure.Storage.Blobs;
    using GreenPipes.Agents;
    using Transports;


    public class EventHubProcessorContextSupervisor :
        TransportPipeContextSupervisor<IEventHubProcessorContext>,
        IEventHubProcessorContextSupervisor
    {
        public EventHubProcessorContextSupervisor(IAgent supervisor, ReceiveSettings receiveSettings, BlobContainerClient blobContainerClient,
            EventProcessorClient client, Func<PartitionClosingEventArgs, Task> partitionClosingHandler,
            Func<PartitionInitializingEventArgs, Task> partitionInitializingHandler)
            : base(supervisor,
                new EventHubProcessorContextFactory(receiveSettings, blobContainerClient, client, partitionClosingHandler, partitionInitializingHandler))
        {
        }
    }
}
