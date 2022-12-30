namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Internals;
    using MassTransit.Configuration;


    public class ProcessorContextFactory :
        IPipeContextFactory<ProcessorContext>
    {
        readonly Func<EventProcessorClient> _clientFactory;
        readonly IConnectionContextSupervisor _contextSupervisor;
        readonly IHostConfiguration _hostConfiguration;
        readonly Func<PartitionClosingEventArgs, Task> _partitionClosingHandler;
        readonly Func<PartitionInitializingEventArgs, Task> _partitionInitializingHandler;

        public ProcessorContextFactory(IConnectionContextSupervisor contextSupervisor, IHostConfiguration hostConfiguration,
            Func<EventProcessorClient> clientFactory,
            Func<PartitionClosingEventArgs, Task> partitionClosingHandler,
            Func<PartitionInitializingEventArgs, Task> partitionInitializingHandler)
        {
            _contextSupervisor = contextSupervisor;
            _hostConfiguration = hostConfiguration;
            _clientFactory = clientFactory;
            _partitionClosingHandler = partitionClosingHandler;
            _partitionInitializingHandler = partitionInitializingHandler;
        }

        public IActivePipeContextAgent<ProcessorContext> CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ProcessorContext> context, CancellationToken cancellationToken = new CancellationToken())
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        IPipeContextAgent<ProcessorContext> IPipeContextFactory<ProcessorContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ProcessorContext> asyncContext = supervisor.AddAsyncContext<ProcessorContext>();

            CreateProcessor(asyncContext, supervisor.Stopped);

            return asyncContext;
        }

        static async Task<ProcessorContext> CreateSharedConnection(Task<ProcessorContext> context,
            CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedProcessorContext(context.Result, cancellationToken)
                : new SharedProcessorContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        void CreateProcessor(IAsyncPipeContextAgent<ProcessorContext> asyncContext, CancellationToken cancellationToken)
        {
            Task<ProcessorContext> Create(ConnectionContext connectionContext, CancellationToken createCancellationToken)
            {
                var client = _clientFactory();
                ProcessorContext context = new EventHubProcessorContext(_hostConfiguration,
                    client, _partitionInitializingHandler, _partitionClosingHandler, createCancellationToken);
                return Task.FromResult(context);
            }

            _contextSupervisor.CreateAgent(asyncContext, Create, cancellationToken);
        }
    }
}
