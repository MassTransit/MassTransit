namespace MassTransit.EventHubIntegration.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Azure.Storage.Blobs;
    using MassTransit.Configuration;
    using Middleware;
    using Transports;


    public class EventHubReceiveEndpointConfigurator :
        ReceiverConfiguration,
        IEventHubReceiveEndpointConfigurator,
        ReceiveSettings
    {
        readonly IBusInstance _busInstance;
        readonly IReceiveEndpointConfiguration _endpointConfiguration;
        readonly IEventHubHostConfiguration _hostConfiguration;
        readonly PipeConfigurator<ProcessorContext> _processorConfigurator;
        Action<EventProcessorClientOptions> _configureOptions;
        string _containerName;
        Func<PartitionClosingEventArgs, Task> _partitionClosingHandler;
        Func<PartitionInitializingEventArgs, Task> _partitionInitializingHandler;

        public EventHubReceiveEndpointConfigurator(IEventHubHostConfiguration hostConfiguration, IBusInstance busInstance,
            IReceiveEndpointConfiguration endpointConfiguration, string eventHubName, string consumerGroup)
            : base(endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _busInstance = busInstance;
            _endpointConfiguration = endpointConfiguration;

            EventHubName = eventHubName;
            ConsumerGroup = consumerGroup;

            ConcurrentMessageLimit = 1;

            CheckpointInterval = TimeSpan.FromMinutes(1);
            CheckpointMessageCount = 5000;
            CheckpointMessageLimit = 10000;

            _processorConfigurator = new PipeConfigurator<ProcessorContext>();
        }

        public string ContainerName
        {
            get => _containerName;
            set => _containerName = value ?? throw new ArgumentNullException(nameof(value));
        }

        public TimeSpan CheckpointInterval { get; set; }
        public ushort CheckpointMessageLimit { get; set; }
        public ushort CheckpointMessageCount { get; set; }

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

        int ReceiveSettings.ConcurrentMessageLimit => Transport.GetConcurrentMessageLimit();

        public string ConsumerGroup { get; }
        public string EventHubName { get; }

        public ReceiveEndpoint Build()
        {
            IEventHubReceiveEndpointContext CreateContext()
            {
                var builder = new EventHubReceiveEndpointBuilder(_hostConfiguration, _busInstance, _endpointConfiguration, this, CreateBlobClient,
                    CreateEventProcessorClient, _partitionClosingHandler, _partitionInitializingHandler);

                foreach (var specification in Specifications)
                    specification.Configure(builder);

                return builder.CreateReceiveEndpointContext();
            }

            var context = CreateContext();

            _processorConfigurator.UseFilter(new EventHubBlobContainerFactoryFilter());
            _processorConfigurator.UseFilter(new EventHubConsumerFilter(context));

            IPipe<ProcessorContext> processorPipe = _processorConfigurator.Build();

            var transport = new ReceiveTransport<ProcessorContext>(_busInstance.HostConfiguration, context, () => context.ContextSupervisor,
                processorPipe);

            return new ReceiveEndpoint(transport, context);
        }

        BlobContainerClient CreateBlobClient(IStorageSettings storageSettings)
        {
            var blobClientOptions = new BlobClientOptions();
            storageSettings.Configure?.Invoke(blobClientOptions);

            var containerName = _containerName ?? EventHubName;
            if (!string.IsNullOrWhiteSpace(storageSettings.ConnectionString))
                return new BlobContainerClient(storageSettings.ConnectionString, containerName, blobClientOptions);

            var uri = new Uri(storageSettings.ContainerUri, containerName);
            if (storageSettings.TokenCredential != null)
                return new BlobContainerClient(uri, storageSettings.TokenCredential, blobClientOptions);

            return storageSettings.SharedKeyCredential != null
                ? new BlobContainerClient(uri, storageSettings.SharedKeyCredential, blobClientOptions)
                : new BlobContainerClient(storageSettings.ContainerUri, blobClientOptions);
        }

        EventProcessorClient CreateEventProcessorClient(IHostSettings hostSettings, BlobContainerClient blobClient)
        {
            var options = new EventProcessorClientOptions();
            _configureOptions?.Invoke(options);

            var client = !string.IsNullOrWhiteSpace(hostSettings.ConnectionString)
                ? new EventProcessorClient(blobClient, ConsumerGroup, hostSettings.ConnectionString, EventHubName, options)
                : new EventProcessorClient(blobClient, ConsumerGroup, hostSettings.FullyQualifiedNamespace, EventHubName, hostSettings.TokenCredential,
                    options);

            return client;
        }
    }
}
