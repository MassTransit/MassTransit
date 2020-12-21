namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Azure.Storage.Blobs;
    using Context;
    using GreenPipes.Agents;
    using Transports;


    public class EventHubProcessorContextSupervisor :
        TransportPipeContextSupervisor<IEventHubProcessorContext>,
        IEventHubProcessorContextSupervisor
    {
        public EventHubProcessorContextSupervisor(IAgent supervisor, ILogContext logContext, ReceiveSettings receiveSettings,
            Func<BlobContainerClient> blobContainerClientFactory,
            Func<BlobContainerClient, EventProcessorClient> clientFactory, Func<PartitionClosingEventArgs, Task> partitionClosingHandler,
            Func<PartitionInitializingEventArgs, Task> partitionInitializingHandler)
            : base(supervisor,
                new EventHubProcessorContextFactory(logContext, receiveSettings, blobContainerClientFactory, clientFactory, partitionClosingHandler,
                    partitionInitializingHandler))
        {
        }
    }
}
