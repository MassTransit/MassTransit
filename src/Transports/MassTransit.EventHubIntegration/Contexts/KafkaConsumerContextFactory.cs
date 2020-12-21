namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;


    public class EventHubProcessorContextFactory :
        IPipeContextFactory<IEventHubProcessorContext>
    {
        readonly ReceiveSettings _receiveSettings;
        readonly Action<EventProcessorClientOptions> _configureOptions;
        readonly Func<PartitionClosingEventArgs, Task> _partitionClosingHandler;
        readonly Func<PartitionInitializingEventArgs, Task> _partitionInitializingHandler;
        readonly IStorageSettings _storageSettings;
        readonly IHostSettings _hostSettings;

        public EventHubProcessorContextFactory(IHostSettings hostSettings, IStorageSettings storageSettings, ReceiveSettings receiveSettings,
            Action<EventProcessorClientOptions> configureOptions, Func<PartitionClosingEventArgs, Task> partitionClosingHandler,
            Func<PartitionInitializingEventArgs, Task> partitionInitializingHandler)
        {
            _receiveSettings = receiveSettings;
            _configureOptions = configureOptions;
            _partitionClosingHandler = partitionClosingHandler;
            _partitionInitializingHandler = partitionInitializingHandler;
            _storageSettings = storageSettings;
            _hostSettings = hostSettings;
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

            return new EventHubProcessorContext(_hostSettings, _storageSettings, _receiveSettings, _configureOptions, _partitionClosingHandler,
                _partitionInitializingHandler, supervisor.Stopping);
        }

        IPipeContextAgent<IEventHubProcessorContext> IPipeContextFactory<IEventHubProcessorContext>.CreateContext(ISupervisor supervisor)
        {
            IEventHubProcessorContext context = CreateProcessor(supervisor);
            IPipeContextAgent<IEventHubProcessorContext> contextHandle = supervisor.AddContext(Task.FromResult(context));

            return contextHandle;
        }
    }
}
