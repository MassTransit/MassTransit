namespace MassTransit.EventHubIntegration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Internals;


    public class ProducerContextFactory :
        IPipeContextFactory<ProducerContext>
    {
        readonly IConnectionContextSupervisor _contextSupervisor;
        readonly string _eventHubName;
        readonly ISerialization _serializers;

        public ProducerContextFactory(IConnectionContextSupervisor contextSupervisor, string eventHubName, ISerialization serializers)
        {
            _contextSupervisor = contextSupervisor;
            _eventHubName = eventHubName;
            _serializers = serializers;
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
                var client = connectionContext.CreateEventHubClient(_eventHubName);
                ProducerContext context = new EventHubProducerContext(client, _serializers, cancellationToken);
                return Task.FromResult(context);
            }

            _contextSupervisor.CreateAgent(asyncContext, Create, cancellationToken);
        }
    }
}
