namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Azure.Storage.Blobs;
    using MassTransit.Configuration;
    using MassTransit.Middleware;


    public class EventHubProcessorContext :
        BasePipeContext,
        ProcessorContext
    {
        readonly BlobContainerClient _blobContainerClient;
        readonly EventProcessorClient _client;
        readonly IProcessorLockContext _lockContext;

        public EventHubProcessorContext(IHostConfiguration hostConfiguration, ReceiveSettings receiveSettings, BlobContainerClient blobContainerClient,
            EventProcessorClient client, Func<PartitionInitializingEventArgs, Task> partitionInitializingHandler,
            Func<PartitionClosingEventArgs, Task> partitionClosingHandler, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _blobContainerClient = blobContainerClient;
            _client = client;

            var lockContext = new ProcessorLockContext(hostConfiguration, receiveSettings);
            _client.PartitionInitializingAsync += async args =>
            {
                await lockContext.OnPartitionInitializing(args).ConfigureAwait(false);
                if (partitionInitializingHandler != null)
                    await partitionInitializingHandler(args).ConfigureAwait(false);
            };
            _client.PartitionClosingAsync += async args =>
            {
                if (partitionClosingHandler != null)
                    await partitionClosingHandler(args).ConfigureAwait(false);
                await lockContext.OnPartitionClosing(args).ConfigureAwait(false);
            };
            _client.ProcessErrorAsync += OnError;
            _client.ProcessEventAsync += OnMessage;

            ReceiveSettings = receiveSettings;
            _lockContext = lockContext;
        }

        public event Func<ProcessEventArgs, Task> ProcessEvent;
        public event Func<ProcessErrorEventArgs, Task> ProcessError;

        public ReceiveSettings ReceiveSettings { get; }

        public async Task<bool> CreateBlobIfNotExistsAsync(CancellationToken cancellationToken = default)
        {
            Response<bool> exists = await _blobContainerClient.ExistsAsync(cancellationToken).ConfigureAwait(false);
            if (exists.Value)
                return true;

            try
            {
                await _blobContainerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
                return true;
            }
            catch (RequestFailedException exception)
            {
                LogContext.Warning?.Log(exception, "Azure Blob Container does not exist: {Address}", _blobContainerClient.Uri);
                return false;
            }
        }

        public Task StartProcessingAsync(CancellationToken cancellationToken = default)
        {
            return _client.StartProcessingAsync(cancellationToken);
        }

        public Task StopProcessingAsync(CancellationToken cancellationToken = default)
        {
            return _client.StopProcessingAsync(cancellationToken);
        }

        public Task Pending(ProcessEventArgs eventArgs)
        {
            return _lockContext.Pending(eventArgs);
        }

        public Task Faulted(ProcessEventArgs eventArgs, Exception exception)
        {
            return _lockContext.Faulted(eventArgs, exception);
        }

        public Task Complete(ProcessEventArgs eventArgs)
        {
            return _lockContext.Complete(eventArgs);
        }

        async Task OnError(ProcessErrorEventArgs arg)
        {
            if (ProcessError != null)
                await ProcessError.Invoke(arg).ConfigureAwait(false);
        }

        async Task OnMessage(ProcessEventArgs arg)
        {
            if (!arg.HasEvent)
                return;

            await _lockContext.Pending(arg).ConfigureAwait(false);

            if (ProcessEvent != null)
                await ProcessEvent.Invoke(arg).ConfigureAwait(false);
        }
    }
}
