namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Azure.Storage.Blobs;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;


    public class EventHubProcessorContextFactory :
        IPipeContextFactory<IEventHubProcessorContext>
    {
        readonly ReceiveSettings _receiveSettings;
        readonly BlobContainerClient _blobContainerClient;
        readonly EventProcessorClient _client;
        readonly Func<PartitionClosingEventArgs, Task> _partitionClosingHandler;
        readonly Func<PartitionInitializingEventArgs, Task> _partitionInitializingHandler;

        public EventHubProcessorContextFactory(ReceiveSettings receiveSettings, BlobContainerClient blobContainerClient, EventProcessorClient client,
            Func<PartitionClosingEventArgs, Task> partitionClosingHandler,
            Func<PartitionInitializingEventArgs, Task> partitionInitializingHandler)
        {
            _receiveSettings = receiveSettings;
            _blobContainerClient = blobContainerClient;
            _client = client;
            _partitionClosingHandler = partitionClosingHandler;
            _partitionInitializingHandler = partitionInitializingHandler;
        }

        public IActivePipeContextAgent<IEventHubProcessorContext> CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<IEventHubProcessorContext> context, CancellationToken cancellationToken = new CancellationToken())
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        static async Task<IEventHubProcessorContext> CreateSharedConnection(Task<IEventHubProcessorContext> context,
            CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedEventHubProcessorContext(context.Result, cancellationToken)
                : new SharedEventHubProcessorContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        IEventHubProcessorContext CreateProcessor(ISupervisor supervisor)
        {
            if (supervisor.Stopping.IsCancellationRequested)
                throw new OperationCanceledException($"The connection is stopping and cannot be used: {_receiveSettings.EventHubName}");

            var context = new EventHubProcessorContext(_receiveSettings, _blobContainerClient, _client,
                supervisor.Stopping);

            context.PartitionInitializing += async args =>
            {
                if (_partitionInitializingHandler != null)
                    await _partitionInitializingHandler(args).ConfigureAwait(false);
            };

            context.PartitionClosing += async args =>
            {
                if (_partitionClosingHandler != null)
                    await _partitionClosingHandler(args).ConfigureAwait(false);
            };

            return context;
        }

        IPipeContextAgent<IEventHubProcessorContext> IPipeContextFactory<IEventHubProcessorContext>.CreateContext(ISupervisor supervisor)
        {
            IEventHubProcessorContext context = CreateProcessor(supervisor);
            IPipeContextAgent<IEventHubProcessorContext> contextHandle = supervisor.AddContext(Task.FromResult(context));

            return contextHandle;
        }
    }
}
