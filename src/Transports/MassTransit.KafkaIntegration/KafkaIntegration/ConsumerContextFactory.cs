namespace MassTransit.KafkaIntegration
{
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Internals;
    using MassTransit.Configuration;


    public class ConsumerContextFactory :
        IPipeContextFactory<ConsumerContext>
    {
        readonly IHostConfiguration _hostConfiguration;
        readonly IClientContextSupervisor _clientContextSupervisor;
        readonly ConsumerBuilderFactory _consumerBuilderFactory;

        public ConsumerContextFactory(IHostConfiguration hostConfiguration, IClientContextSupervisor clientContextSupervisor,
            ConsumerBuilderFactory consumerBuilderFactory)
        {
            _hostConfiguration = hostConfiguration;
            _clientContextSupervisor = clientContextSupervisor;
            _consumerBuilderFactory = consumerBuilderFactory;
        }

        public IPipeContextAgent<ConsumerContext> CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ConsumerContext> asyncContext = supervisor.AddAsyncContext<ConsumerContext>();

            CreateConsumer(asyncContext, supervisor.Stopped);

            return asyncContext;
        }

        public IActivePipeContextAgent<ConsumerContext> CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ConsumerContext> context, CancellationToken cancellationToken = new CancellationToken())
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        static async Task<ConsumerContext> CreateSharedConnection(Task<ConsumerContext> context,
            CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedConsumerContext(context.Result, cancellationToken)
                : new SharedConsumerContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        void CreateConsumer(IAsyncPipeContextAgent<ConsumerContext> asyncContext, CancellationToken cancellationToken)
        {
            Task<ConsumerContext> Create(ClientContext clientContext, CancellationToken createCancellationToken)
            {
                ConsumerContext context = new KafkaConsumerContext(_hostConfiguration, _consumerBuilderFactory, cancellationToken);

                return Task.FromResult(context);
            }

            _clientContextSupervisor.CreateAgent(asyncContext, Create, cancellationToken);
        }
    }
}
