using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using GreenPipes;
using GreenPipes.Agents;
using GreenPipes.Internals.Extensions;
using MassTransit.Configuration;
using MassTransit.Internals.Extensions;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class ProcessorContextFactory :
        IPipeContextFactory<ProcessorContext>
    {
        //readonly Func<ICheckpointStore> _checkpointStoreFactory;
        readonly Func<IHostSettings, ICheckpointStore, EventStoreClient> _clientFactory;
        readonly IClientContextSupervisor _contextSupervisor;
        readonly IHostConfiguration _hostConfiguration;
        readonly ReceiveSettings _receiveSettings;

        public ProcessorContextFactory(IClientContextSupervisor contextSupervisor, IHostConfiguration hostConfiguration,
            ReceiveSettings receiveSettings, Func<IHostSettings, ICheckpointStore, EventStoreClient> clientFactory)
        {
            _contextSupervisor = contextSupervisor;
            _hostConfiguration = hostConfiguration;
            _receiveSettings = receiveSettings;
            _clientFactory = clientFactory;
        }

        public IActivePipeContextAgent<ProcessorContext> CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ProcessorContext> context, CancellationToken cancellationToken = default)
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }


        IPipeContextAgent<ProcessorContext> IPipeContextFactory<ProcessorContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ProcessorContext> asyncContext = supervisor.AddAsyncContext<ProcessorContext>();

            CreateProcessor(asyncContext, supervisor.Stopped);

            return asyncContext;
        }


        static async Task<ProcessorContext> CreateSharedConnection(Task<ProcessorContext> context, CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedProcessorContext(context.Result, cancellationToken)
                : new SharedProcessorContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        void CreateProcessor(IAsyncPipeContextAgent<ProcessorContext> asyncContext, CancellationToken cancellationToken)
        {
            Task<ProcessorContext> Create(ClientContext connectionContext, CancellationToken createCancellationToken)
            {
                //var client = _clientFactory(connectionContext.HostSettings, blobContainerClient);
                //ProcessorContext context = new EventStoreDbProcessorContext(_hostConfiguration, _receiveSettings, checkpointStore,
                //    client, _partitionInitializingHandler, _partitionClosingHandler, createCancellationToken);
                //return Task.FromResult(context);
                throw new NotImplementedException();
            }

            _contextSupervisor.CreateAgent(asyncContext, Create, cancellationToken);
        }
    }
}
