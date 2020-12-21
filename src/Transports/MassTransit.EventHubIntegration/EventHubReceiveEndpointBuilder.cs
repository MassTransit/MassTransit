namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Consumer;
    using Azure.Messaging.EventHubs.Processor;
    using Azure.Storage.Blobs;
    using Builders;
    using Configuration;
    using MassTransit.Registration;


    public class EventHubReceiveEndpointBuilder :
        ReceiveEndpointBuilder
    {
        readonly IBusInstance _busInstance;
        readonly IReceiveEndpointConfiguration _configuration;
        readonly ReceiveSettings _receiveSettings;
        readonly Func<BlobContainerClient> _blobContainerClientFactory;
        readonly Func<BlobContainerClient, EventProcessorClient> _clientFactory;
        readonly Func<PartitionClosingEventArgs, Task> _partitionClosingHandler;
        readonly Func<PartitionInitializingEventArgs, Task> _partitionInitializingHandler;

        public EventHubReceiveEndpointBuilder(IBusInstance busInstance, IReceiveEndpointConfiguration configuration, ReceiveSettings receiveSettings,
            Func<BlobContainerClient> blobContainerClientFactory, Func<BlobContainerClient, EventProcessorClient> clientFactory,
            Func<PartitionClosingEventArgs, Task> partitionClosingHandler,
            Func<PartitionInitializingEventArgs, Task> partitionInitializingHandler)
            : base(configuration)
        {
            _busInstance = busInstance;
            _configuration = configuration;
            _receiveSettings = receiveSettings;
            _blobContainerClientFactory = blobContainerClientFactory;
            _clientFactory = clientFactory;
            _partitionClosingHandler = partitionClosingHandler;
            _partitionInitializingHandler = partitionInitializingHandler;
        }

        public IEventHubReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var context = new EventHubReceiveEndpointContext(_busInstance, _configuration, _receiveSettings, _blobContainerClientFactory, _clientFactory,
                _partitionClosingHandler, _partitionInitializingHandler);

            context.GetOrAddPayload(() => _busInstance.HostConfiguration.HostTopology);
            context.AddOrUpdatePayload(() => _receiveSettings, _ => _receiveSettings);

            return context;
        }
    }
}
