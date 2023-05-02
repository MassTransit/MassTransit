namespace MassTransit.EventHubIntegration.Configuration
{
    using System;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Azure.Storage.Blobs;
    using MassTransit.Configuration;
    using MassTransit.Middleware;
    using Middleware;
    using Transports;


    public class EventHubReceiveEndpointConfigurator :
        ReceiveEndpointConfiguration,
        IEventHubReceiveEndpointConfigurator,
        ReceiveSettings
    {
        readonly Lazy<BlobContainerClient> _blobClient;
        readonly IBusInstance _busInstance;
        readonly IReceiveEndpointConfiguration _endpointConfiguration;
        readonly IEventHubHostConfiguration _hostConfiguration;
        readonly IHostSettings _hostSettings;
        readonly PipeConfigurator<ProcessorContext> _processorConfigurator;
        readonly IStorageSettings _storageSettings;
        Action<EventProcessorClientOptions> _configureOptions;
        string _containerName;
        Func<PartitionClosingEventArgs, Task> _partitionClosingHandler;
        Func<PartitionInitializingEventArgs, Task> _partitionInitializingHandler;

        public EventHubReceiveEndpointConfigurator(IEventHubHostConfiguration hostConfiguration, IBusInstance busInstance,
            IReceiveEndpointConfiguration endpointConfiguration, IHostSettings hostSettings, IStorageSettings storageSettings, string eventHubName,
            string consumerGroup)
            : base(busInstance.HostConfiguration, endpointConfiguration)
        {
            _hostConfiguration = hostConfiguration;
            _busInstance = busInstance;
            _endpointConfiguration = endpointConfiguration;
            _hostSettings = hostSettings;
            _storageSettings = storageSettings;

            EventHubName = eventHubName;
            ConsumerGroup = consumerGroup;

            ConcurrentMessageLimit = 1;
            ConcurrentDeliveryLimit = 1;

            CheckpointInterval = TimeSpan.FromMinutes(1);
            CheckpointMessageCount = 5000;
            CheckpointMessageLimit = 10000;

            PrefetchCount = Math.Max(1000, CheckpointMessageCount / 10);

            _processorConfigurator = new PipeConfigurator<ProcessorContext>();
            _blobClient = new Lazy<BlobContainerClient>(CreateBlobClient);

            PublishFaults = false;

            this.DiscardFaultedMessages();
            this.DiscardSkippedMessages();
        }

        public override Uri HostAddress => _endpointConfiguration.HostAddress;

        public string ContainerName
        {
            get => _containerName;
            set
            {
                _containerName = value ?? throw new ArgumentNullException(nameof(value));

                Changed(nameof(ContainerName));
            }
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

        public override Uri InputAddress => _endpointConfiguration.InputAddress;
        public int ConcurrentDeliveryLimit { get; set; }

        int ReceiveSettings.ConcurrentMessageLimit => Transport.GetConcurrentMessageLimit();

        public string ConsumerGroup { get; }
        public string EventHubName { get; }

        public override ReceiveEndpointContext CreateReceiveEndpointContext()
        {
            return CreateEventHubReceiveContext();
        }

        public ReceiveEndpoint Build()
        {
            var context = CreateEventHubReceiveContext();

            _processorConfigurator.UseFilter(new EventHubBlobContainerFactoryFilter(_blobClient.Value));
            _processorConfigurator.UseFilter(new ReceiveEndpointDependencyFilter<ProcessorContext>(context));
            _processorConfigurator.UseFilter(new EventHubConsumerFilter(context));

            IPipe<ProcessorContext> processorPipe = _processorConfigurator.Build();

            var transport = new ReceiveTransport<ProcessorContext>(_busInstance.HostConfiguration, context, () => context.ContextSupervisor,
                processorPipe);

            return new ReceiveEndpoint(transport, context);
        }

        IEventHubReceiveEndpointContext CreateEventHubReceiveContext()
        {
            var builder = new EventHubReceiveEndpointBuilder(_hostConfiguration, _busInstance, this, this,
                CreateEventProcessorClient, _partitionClosingHandler, _partitionInitializingHandler);

            ApplySpecifications(builder);

            return builder.CreateReceiveEndpointContext();
        }

        BlobContainerClient CreateBlobClient()
        {
            var blobClientOptions = new BlobClientOptions();
            _storageSettings.Configure?.Invoke(blobClientOptions);

            var containerName = _containerName ?? EventHubName;
            if (!string.IsNullOrWhiteSpace(_storageSettings.ConnectionString))
                return new BlobContainerClient(_storageSettings.ConnectionString, containerName, blobClientOptions);

            var uri = new Uri(_storageSettings.ContainerUri, containerName);
            if (_storageSettings.TokenCredential != null)
                return new BlobContainerClient(uri, _storageSettings.TokenCredential, blobClientOptions);

            return _storageSettings.SharedKeyCredential != null
                ? new BlobContainerClient(uri, _storageSettings.SharedKeyCredential, blobClientOptions)
                : new BlobContainerClient(_storageSettings.ContainerUri, blobClientOptions);
        }

        EventProcessorClient CreateEventProcessorClient()
        {
            var options = new EventProcessorClientOptions();
            _configureOptions?.Invoke(options);

            var client = !string.IsNullOrWhiteSpace(_hostSettings.ConnectionString)
                ? new EventProcessorClient(_blobClient.Value, ConsumerGroup, _hostSettings.ConnectionString, EventHubName, options)
                : new EventProcessorClient(_blobClient.Value, ConsumerGroup, _hostSettings.FullyQualifiedNamespace, EventHubName, _hostSettings.TokenCredential,
                    options);

            return client;
        }

        protected override bool IsAlreadyConfigured()
        {
            return _blobClient.IsValueCreated || base.IsAlreadyConfigured();
        }
    }
}
