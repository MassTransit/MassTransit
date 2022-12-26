namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Agents;
    using Confluent.Kafka;
    using Internals;
    using MassTransit.Configuration;


    public class ProducerContextFactory :
        IPipeContextFactory<ProducerContext>
    {
        readonly IClientContextSupervisor _clientContextSupervisor;
        readonly IHostConfiguration _hostConfiguration;
        readonly Func<ProducerBuilder<byte[], byte[]>> _producerBuilderFactory;

        public ProducerContextFactory(IClientContextSupervisor clientContextSupervisor, IHostConfiguration hostConfiguration,
            Func<ProducerBuilder<byte[], byte[]>> producerBuilderFactory)
        {
            _clientContextSupervisor = clientContextSupervisor;
            _hostConfiguration = hostConfiguration;
            _producerBuilderFactory = producerBuilderFactory;
        }

        public IPipeContextAgent<ProducerContext> CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ProducerContext> asyncContext = supervisor.AddAsyncContext<ProducerContext>();

            CreateProducer(asyncContext, supervisor.Stopped);

            return asyncContext;
        }

        public IActivePipeContextAgent<ProducerContext> CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ProducerContext> context, CancellationToken cancellationToken = new CancellationToken())
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        static async Task<ProducerContext> CreateSharedConnection(Task<ProducerContext> context,
            CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedProducerContext(context.Result, cancellationToken)
                : new SharedProducerContext(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        void CreateProducer(IAsyncPipeContextAgent<ProducerContext> asyncContext, CancellationToken cancellationToken)
        {
            Task<ProducerContext> Create(ClientContext clientContext, CancellationToken createCancellationToken)
            {
                ProducerBuilder<byte[], byte[]> producerBuilder = _producerBuilderFactory();
                ProducerContext context =
                    new KafkaProducerContext(producerBuilder, _hostConfiguration.SendLogContext, cancellationToken);
                return Task.FromResult(context);
            }

            _clientContextSupervisor.CreateAgent(asyncContext, Create, cancellationToken);
        }
    }
}
