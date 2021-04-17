using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using GreenPipes.Agents;
using GreenPipes.Internals.Extensions;
using MassTransit.Configuration;
using MassTransit.EventStoreDbIntegration.Serializers;
using MassTransit.Internals.Extensions;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class ProcessorContextFactory :
        IPipeContextFactory<ProcessorContext>
    {
        readonly CheckpointStoreFactory _checkpointStoreFactory;
        readonly IConnectionContextSupervisor _contextSupervisor;
        readonly IHeadersDeserializer _headersDeserializer;
        readonly IHostConfiguration _hostConfiguration;
        readonly ReceiveSettings _receiveSettings;

        public ProcessorContextFactory(IConnectionContextSupervisor contextSupervisor, IHostConfiguration hostConfiguration,
            ReceiveSettings receiveSettings, IHeadersDeserializer headersDeserializer,
            CheckpointStoreFactory checkpointStoreFactory)
        {
            _contextSupervisor = contextSupervisor;
            _hostConfiguration = hostConfiguration;
            _receiveSettings = receiveSettings;
            _headersDeserializer = headersDeserializer;
            _checkpointStoreFactory = checkpointStoreFactory;
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
            Task<ProcessorContext> Create(ConnectionContext connectionContext, CancellationToken createCancellationToken)
            {
                var client = connectionContext.CreateEventStoreDbClient();
                var checkpointStore = _checkpointStoreFactory(client);
                ProcessorContext context = new EventStoreDbProcessorContext(_hostConfiguration, _receiveSettings, client, _headersDeserializer,
                    checkpointStore, createCancellationToken);
                return Task.FromResult(context);
            }

            _contextSupervisor.CreateAgent(asyncContext, Create, cancellationToken);
        }
    }
}
