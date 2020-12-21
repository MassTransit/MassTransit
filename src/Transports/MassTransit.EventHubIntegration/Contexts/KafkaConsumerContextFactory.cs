namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Azure.Storage.Blobs;
    using Context;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;


    public class EventHubProcessorContextFactory :
        IPipeContextFactory<IEventHubProcessorContext>
    {
        readonly Func<BlobContainerClient> _blobContainerClientFactory;
        readonly Func<BlobContainerClient, EventProcessorClient> _clientFactory;
        readonly ILogContext _logContext;
        readonly Func<PartitionClosingEventArgs, Task> _partitionClosingHandler;
        readonly Func<PartitionInitializingEventArgs, Task> _partitionInitializingHandler;
        readonly ReceiveSettings _receiveSettings;

        public EventHubProcessorContextFactory(ILogContext logContext, ReceiveSettings receiveSettings, Func<BlobContainerClient> blobContainerClientFactory,
            Func<BlobContainerClient, EventProcessorClient> clientFactory,
            Func<PartitionClosingEventArgs, Task> partitionClosingHandler,
            Func<PartitionInitializingEventArgs, Task> partitionInitializingHandler)
        {
            _logContext = logContext;
            _receiveSettings = receiveSettings;
            _blobContainerClientFactory = blobContainerClientFactory;
            _clientFactory = clientFactory;
            _partitionClosingHandler = partitionClosingHandler;
            _partitionInitializingHandler = partitionInitializingHandler;
        }

        public IActivePipeContextAgent<IEventHubProcessorContext> CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<IEventHubProcessorContext> context, CancellationToken cancellationToken = new CancellationToken())
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        IPipeContextAgent<IEventHubProcessorContext> IPipeContextFactory<IEventHubProcessorContext>.CreateContext(ISupervisor supervisor)
        {
            var context = CreateProcessor(supervisor);
            IPipeContextAgent<IEventHubProcessorContext> contextHandle = supervisor.AddContext(Task.FromResult(context));

            return contextHandle;
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

            var blobContainerClient = _blobContainerClientFactory();
            return new EventHubProcessorContext(_logContext, _receiveSettings, blobContainerClient, _clientFactory(blobContainerClient),
                _partitionInitializingHandler, _partitionClosingHandler,
                supervisor.Stopping);
        }
    }
}
