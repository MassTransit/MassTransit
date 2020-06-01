namespace MassTransit.EventHubIntegration.Configurators
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Azure.Storage.Blobs;
    using Configuration;
    using Context;
    using Contexts;
    using GreenPipes;
    using MassTransit.Registration;
    using Riders;
    using Transport;


    public class EventHubConfigurator :
        ReceiverConfiguration,
        IEventHubConfigurator
    {
        readonly IBusInstance _busInstance;
        readonly string _consumerGroup;
        readonly IReceiveEndpointConfiguration _endpointConfiguration;
        readonly string _eventHubName;
        readonly IHostSettings _hostSettings;
        readonly IStorageSettings _storageSettings;
        Action<EventProcessorClientOptions> _configureOptions;
        string _containerName;
        Func<PartitionClosingEventArgs, Task> _partitionClosingHandler;
        Func<PartitionInitializingEventArgs, Task> _partitionInitializingHandler;

        public EventHubConfigurator(string eventHubName, string consumerGroup, IHostSettings hostSettings, IStorageSettings storageSettings,
            IBusInstance busInstance,
            IReceiveEndpointConfiguration endpointConfiguration)
            : base(endpointConfiguration)
        {
            _eventHubName = eventHubName;
            _consumerGroup = consumerGroup;
            _hostSettings = hostSettings;
            _storageSettings = storageSettings;
            _busInstance = busInstance;
            _endpointConfiguration = endpointConfiguration;
        }

        public string ContainerName
        {
            set => _containerName = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Action<EventProcessorClientOptions> ConfigureOptions
        {
            set => _configureOptions = value ?? throw new ArgumentNullException(nameof(value));
        }

        public void OnPartitionClosing(Func<PartitionClosingEventArgs, Task> handler)
        {
            if (_partitionClosingHandler != null)
                throw new InvalidOperationException("Partition closing event handler may not be specified more than once.");
            _partitionClosingHandler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public void OnPartitionInitializing(Func<PartitionInitializingEventArgs, Task> handler)
        {
            if (_partitionInitializingHandler != null)
                throw new InvalidOperationException("Partition initializing event handler may not be specified more than once.");
            _partitionInitializingHandler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public IEventHubReceiveEndpoint Build()
        {
            ReceiveEndpointContext CreateContext()
            {
                var builder = new RiderReceiveEndpointBuilder(_busInstance, _endpointConfiguration);

                foreach (var specification in Specifications)
                    specification.Configure(builder);

                return builder.CreateReceiveEndpointContext();
            }

            var context = CreateContext();
            var blobClient = CreateBlobClient();
            var processor = CreateEventProcessorClient(blobClient);
            var transport = new EventHubReceiveTransport(context);

            AddPayload(context);

            return new EventHubReceiveEndpoint(processor, blobClient, transport, context);
        }

        BlobContainerClient CreateBlobClient()
        {
            var blobClientOptions = new BlobClientOptions();
            _storageSettings.Configure?.Invoke(blobClientOptions);

            if (!string.IsNullOrWhiteSpace(_storageSettings.ConnectionString))
                return new BlobContainerClient(_storageSettings.ConnectionString, _containerName ?? _eventHubName, blobClientOptions);
            if (_storageSettings.TokenCredential != null)
                return new BlobContainerClient(_storageSettings.ContainerUri, _storageSettings.TokenCredential, blobClientOptions);

            return _storageSettings.SharedKeyCredential != null
                ? new BlobContainerClient(_storageSettings.ContainerUri, _storageSettings.SharedKeyCredential, blobClientOptions)
                : new BlobContainerClient(_storageSettings.ContainerUri, blobClientOptions);
        }

        EventProcessorClient CreateEventProcessorClient(BlobContainerClient blobClient)
        {
            var options = new EventProcessorClientOptions();
            _configureOptions?.Invoke(options);

            var client = !string.IsNullOrWhiteSpace(_hostSettings.ConnectionString)
                ? new EventProcessorClient(blobClient, _consumerGroup, _hostSettings.ConnectionString, _eventHubName, options)
                : new EventProcessorClient(blobClient, _consumerGroup, _hostSettings.FullyQualifiedNamespace, _eventHubName, _hostSettings.TokenCredential,
                    options);

            if (_partitionClosingHandler != null)
                client.PartitionClosingAsync += _partitionClosingHandler;
            if (_partitionInitializingHandler != null)
                client.PartitionInitializingAsync += _partitionInitializingHandler;

            return client;
        }

        static void AddPayload(PipeContext context)
        {
            IProcessorLockContext lockContext = new ProcessorLockContext();
            context.AddOrUpdatePayload(() => lockContext, _ => lockContext);
        }
    }
}
