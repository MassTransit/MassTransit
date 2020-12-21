namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Azure.Storage.Blobs;
    using Context;
    using GreenPipes;


    public class EventHubProcessorContext :
        BasePipeContext,
        IEventHubProcessorContext
    {
        readonly Action<EventProcessorClientOptions> _configureOptions;
        readonly BlobContainerClient _blobClient;
        readonly Func<PartitionClosingEventArgs, Task> _partitionClosingHandler;
        readonly Func<PartitionInitializingEventArgs, Task> _partitionInitializingHandler;

        public EventHubProcessorContext(IHostSettings hostSettings, IStorageSettings storageSettings, ReceiveSettings receiveSettings,
            Action<EventProcessorClientOptions> configureOptions, Func<PartitionClosingEventArgs, Task> partitionClosingHandler,
            Func<PartitionInitializingEventArgs, Task> partitionInitializingHandler, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _configureOptions = configureOptions;
            _partitionClosingHandler = partitionClosingHandler;
            _partitionInitializingHandler = partitionInitializingHandler;
            HostSettings = hostSettings;
            StorageSettings = storageSettings;
            ReceiveSettings = receiveSettings;

            _blobClient = CreateBlobClient();
        }

        public IHostSettings HostSettings { get; }

        public IStorageSettings StorageSettings { get; }

        public ReceiveSettings ReceiveSettings { get; }

        public async Task InitializePartition(PartitionInitializingEventArgs eventArgs)
        {
            if (_partitionInitializingHandler != null)
                await _partitionInitializingHandler(eventArgs).ConfigureAwait(false);
        }

        public async Task ClosePartition(PartitionClosingEventArgs eventArgs)
        {
            if (_partitionClosingHandler != null)
                await _partitionClosingHandler(eventArgs).ConfigureAwait(false);
        }

        public async Task<bool> TryCreateContainerIfNotExists(CancellationToken cancellationToken)
        {
            try
            {
                await _blobClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (RequestFailedException exception)
            {
                LogContext.Warning?.Log(exception, "Azure Blob Container does not exist: {Address}", _blobClient.Uri);
            }

            return false;
        }

        public EventProcessorClient CreateClient()
        {
            var options = new EventProcessorClientOptions();
            _configureOptions?.Invoke(options);

            var client = !string.IsNullOrWhiteSpace(HostSettings.ConnectionString)
                ? new EventProcessorClient(_blobClient, ReceiveSettings.ConsumerGroup, HostSettings.ConnectionString, ReceiveSettings.EventHubName, options)
                : new EventProcessorClient(_blobClient, ReceiveSettings.ConsumerGroup, HostSettings.FullyQualifiedNamespace, ReceiveSettings.EventHubName,
                    HostSettings.TokenCredential,
                    options);

            return client;
        }

        BlobContainerClient CreateBlobClient()
        {
            var blobClientOptions = new BlobClientOptions();
            StorageSettings.Configure?.Invoke(blobClientOptions);

            var containerName = ReceiveSettings.ContainerName ?? ReceiveSettings.EventHubName;
            if (!string.IsNullOrWhiteSpace(StorageSettings.ConnectionString))
                return new BlobContainerClient(StorageSettings.ConnectionString, containerName, blobClientOptions);

            var uri = new Uri(StorageSettings.ContainerUri, containerName);
            if (StorageSettings.TokenCredential != null)
                return new BlobContainerClient(uri, StorageSettings.TokenCredential, blobClientOptions);

            return StorageSettings.SharedKeyCredential != null
                ? new BlobContainerClient(uri, StorageSettings.SharedKeyCredential, blobClientOptions)
                : new BlobContainerClient(StorageSettings.ContainerUri, blobClientOptions);
        }
    }
}
