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
    using MassTransit.Registration;
    using Riders;


    public class EventHubReceiveEndpointConfigurator :
        ReceiverConfiguration,
        IEventHubReceiveEndpointConfigurator
    {
        readonly IBusInstance _busInstance;
        readonly string _consumerGroup;
        readonly IReceiveEndpointConfiguration _endpointConfiguration;
        readonly string _eventHubName;
        readonly IHostSettings _hostSettings;
        readonly IStorageSettings _storageSettings;
        TimeSpan _checkpointInterval;
        ushort _checkpointMessageCount;
        int _concurrencyLimit;
        Action<EventProcessorClientOptions> _configureOptions;
        string _containerName;
        Func<PartitionClosingEventArgs, Task> _partitionClosingHandler;
        Func<PartitionInitializingEventArgs, Task> _partitionInitializingHandler;

        public EventHubReceiveEndpointConfigurator(string eventHubName, string consumerGroup, IHostSettings hostSettings, IStorageSettings storageSettings,
            IBusInstance busInstance,
            IReceiveEndpointConfiguration endpointConfiguration)
            : base(busInstance.HostConfiguration, endpointConfiguration)
        {
            _eventHubName = eventHubName;
            _consumerGroup = consumerGroup;
            _hostSettings = hostSettings;
            _storageSettings = storageSettings;
            _busInstance = busInstance;
            _endpointConfiguration = endpointConfiguration;

            CheckpointInterval = TimeSpan.FromMinutes(1);
            CheckpointMessageCount = 5000;
            ConcurrencyLimit = 1;
        }

        public string ContainerName
        {
            set => _containerName = value ?? throw new ArgumentNullException(nameof(value));
        }

        public TimeSpan CheckpointInterval
        {
            set => _checkpointInterval = value;
        }

        public ushort CheckpointMessageCount
        {
            set => _checkpointMessageCount = value;
        }

        public int ConcurrencyLimit
        {
            set => _concurrencyLimit = value;
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

            var lockContext = new ProcessorLockContext(processor, context.LogContext, _checkpointInterval, _checkpointMessageCount,
                _partitionInitializingHandler, _partitionClosingHandler);
            var transport = new EventHubDataReceiver(context, lockContext);

            return new EventHubReceiveEndpoint(processor, Math.Max(1000, _checkpointMessageCount / 10), _concurrencyLimit, blobClient, transport, context);
        }

        BlobContainerClient CreateBlobClient()
        {
            var blobClientOptions = new BlobClientOptions();
            _storageSettings.Configure?.Invoke(blobClientOptions);

            var containerName = _containerName ?? _eventHubName;
            if (!string.IsNullOrWhiteSpace(_storageSettings.ConnectionString))
                return new BlobContainerClient(_storageSettings.ConnectionString, containerName, blobClientOptions);

            var uri = new Uri(_storageSettings.ContainerUri, new Uri(containerName));
            if (_storageSettings.TokenCredential != null)
                return new BlobContainerClient(uri, _storageSettings.TokenCredential, blobClientOptions);

            return _storageSettings.SharedKeyCredential != null
                ? new BlobContainerClient(uri, _storageSettings.SharedKeyCredential, blobClientOptions)
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

            return client;
        }
    }
}
