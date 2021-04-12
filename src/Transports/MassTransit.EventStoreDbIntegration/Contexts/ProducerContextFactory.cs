using System.Threading;
using System.Threading.Tasks;
using GreenPipes;
using GreenPipes.Agents;
using GreenPipes.Internals.Extensions;
using MassTransit.Internals.Extensions;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class ProducerContextFactory :
        IPipeContextFactory<ProducerContext>
    {
        readonly IConnectionContextSupervisor _contextSupervisor;
        readonly IMessageSerializer _messageSerializer;

        public ProducerContextFactory(IConnectionContextSupervisor contextSupervisor, IMessageSerializer messageSerializer)
        {
            _contextSupervisor = contextSupervisor;
            _messageSerializer = messageSerializer;
        }

        public IActivePipeContextAgent<ProducerContext> CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ProducerContext> context, CancellationToken cancellationToken = new CancellationToken())
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        IPipeContextAgent<ProducerContext> IPipeContextFactory<ProducerContext>.CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ProducerContext> asyncContext = supervisor.AddAsyncContext<ProducerContext>();

            CreateProcessor(asyncContext, supervisor.Stopped);

            return asyncContext;
        }

        static async Task<ProducerContext> CreateSharedConnection(Task<ProducerContext> context,
            CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedProducerContext(context.Result, cancellationToken)
                : new SharedProducerContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        void CreateProcessor(IAsyncPipeContextAgent<ProducerContext> asyncContext, CancellationToken cancellationToken)
        {
            Task<ProducerContext> Create(ConnectionContext connectionContext, CancellationToken createCancellationToken)
            {
                var client = connectionContext.CreateEventStoreDbClient();
                ProducerContext context = new EventStoreDbProducerContext(client, _messageSerializer, cancellationToken);
                return Task.FromResult(context);
            }

            _contextSupervisor.CreateAgent(asyncContext, Create, cancellationToken);
        }
    }
}
