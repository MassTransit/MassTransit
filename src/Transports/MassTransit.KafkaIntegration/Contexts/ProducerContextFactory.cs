namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Internals.Extensions;
    using Serializers;
    using Transport;


    public class ProducerContextFactory<TKey, TValue> :
        IPipeContextFactory<ProducerContext<TKey, TValue>>
        where TValue : class
    {
        readonly IClientContextSupervisor _clientContextSupervisor;
        readonly IHeadersSerializer _headersSerializer;
        readonly Func<ProducerBuilder<TKey, TValue>> _producerBuilderFactory;

        public ProducerContextFactory(IClientContextSupervisor clientContextSupervisor, IHeadersSerializer headersSerializer,
            Func<ProducerBuilder<TKey, TValue>> producerBuilderFactory)
        {
            _clientContextSupervisor = clientContextSupervisor;
            _headersSerializer = headersSerializer;
            _producerBuilderFactory = producerBuilderFactory;
        }

        public IPipeContextAgent<ProducerContext<TKey, TValue>> CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ProducerContext<TKey, TValue>> asyncContext = supervisor.AddAsyncContext<ProducerContext<TKey, TValue>>();

            CreateProducer(asyncContext, supervisor.Stopped);

            return asyncContext;
        }

        public IActivePipeContextAgent<ProducerContext<TKey, TValue>> CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ProducerContext<TKey, TValue>> context, CancellationToken cancellationToken = new CancellationToken())
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        static async Task<ProducerContext<TKey, TValue>> CreateSharedConnection(Task<ProducerContext<TKey, TValue>> context,
            CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedProducerContext<TKey, TValue>(context.Result, cancellationToken)
                : new SharedProducerContext<TKey, TValue>(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        void CreateProducer(IAsyncPipeContextAgent<ProducerContext<TKey, TValue>> asyncContext, CancellationToken cancellationToken)
        {
            Task<ProducerContext<TKey, TValue>> Create(ClientContext clientContext, CancellationToken createCancellationToken)
            {
                ProducerBuilder<TKey, TValue> producerBuilder = _producerBuilderFactory();
                ProducerContext<TKey, TValue> context =
                    new KafkaProducerContext<TKey, TValue>(producerBuilder, _headersSerializer, cancellationToken);
                return Task.FromResult(context);
            }

            _clientContextSupervisor.CreateAgent(asyncContext, Create, cancellationToken);
        }
    }
}
