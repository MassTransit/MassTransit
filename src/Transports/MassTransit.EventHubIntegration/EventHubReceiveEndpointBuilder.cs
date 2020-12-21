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
        readonly IHostSettings _hostSettings;
        readonly IStorageSettings _storageSettings;
        readonly ReceiveSettings _receiveSettings;
        readonly Action<EventProcessorClientOptions> _configureOptions;
        readonly Func<PartitionClosingEventArgs, Task> _partitionClosingHandler;
        readonly Func<PartitionInitializingEventArgs, Task> _partitionInitializingHandler;
        readonly Func<BlobContainerClient, EventHubConsumerClient> _clientFactory;

        public EventHubReceiveEndpointBuilder(IBusInstance busInstance, IReceiveEndpointConfiguration configuration,
            IHostSettings hostSettings, IStorageSettings storageSettings, ReceiveSettings receiveSettings,
            Action<EventProcessorClientOptions> configureOptions, Func<PartitionClosingEventArgs, Task> partitionClosingHandler,
            Func<PartitionInitializingEventArgs, Task> partitionInitializingHandler)
            : base(configuration)
        {
            _busInstance = busInstance;
            _configuration = configuration;
            _hostSettings = hostSettings;
            _storageSettings = storageSettings;
            _receiveSettings = receiveSettings;
            _configureOptions = configureOptions;
            _partitionClosingHandler = partitionClosingHandler;
            _partitionInitializingHandler = partitionInitializingHandler;
        }

        public IEventHubReceiveEndpointContext CreateReceiveEndpointContext()
        {
            var context = new EventHubReceiveEndpointContext(_busInstance, _configuration, _hostSettings, _storageSettings, _receiveSettings, _configureOptions,
                _partitionClosingHandler, _partitionInitializingHandler);

            context.GetOrAddPayload(() => _busInstance.HostConfiguration.HostTopology);
            context.AddOrUpdatePayload(() => _receiveSettings, _ => _receiveSettings);

            return context;
        }
    }
}
