namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Configurators;
    using Confluent.Kafka;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Internals.Extensions;
    using Serializers;
    using Transport;


    public class ConsumerContextFactory<TKey, TValue> :
        IPipeContextFactory<ConsumerContext<TKey, TValue>>
        where TValue : class
    {
        readonly CheckpointPipeConfiguration _checkpointPipeConfiguration;
        readonly IClientContextSupervisor _clientContextSupervisor;
        readonly Func<ConsumerBuilder<TKey, TValue>> _consumerBuilderFactory;
        readonly IHeadersDeserializer _headersDeserializer;
        readonly IHostConfiguration _hostConfiguration;
        readonly ReceiveSettings _receiveSettings;

        public ConsumerContextFactory(IClientContextSupervisor clientContextSupervisor, ReceiveSettings receiveSettings,
            IHostConfiguration hostConfiguration, IHeadersDeserializer headersDeserializer, Func<ConsumerBuilder<TKey, TValue>> consumerBuilderFactory,
            CheckpointPipeConfiguration checkpointPipeConfiguration)
        {
            _clientContextSupervisor = clientContextSupervisor;
            _receiveSettings = receiveSettings;
            _headersDeserializer = headersDeserializer;
            _hostConfiguration = hostConfiguration;
            _consumerBuilderFactory = consumerBuilderFactory;
            _checkpointPipeConfiguration = checkpointPipeConfiguration;
        }

        public IPipeContextAgent<ConsumerContext<TKey, TValue>> CreateContext(ISupervisor supervisor)
        {
            IAsyncPipeContextAgent<ConsumerContext<TKey, TValue>> asyncContext = supervisor.AddAsyncContext<ConsumerContext<TKey, TValue>>();

            CreateConsumer(asyncContext, supervisor.Stopped);

            return asyncContext;
        }

        public IActivePipeContextAgent<ConsumerContext<TKey, TValue>> CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<ConsumerContext<TKey, TValue>> context, CancellationToken cancellationToken = new CancellationToken())
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        static async Task<ConsumerContext<TKey, TValue>> CreateSharedConnection(Task<ConsumerContext<TKey, TValue>> context,
            CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedConsumerContext<TKey, TValue>(context.Result, cancellationToken)
                : new SharedConsumerContext<TKey, TValue>(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        void CreateConsumer(IAsyncPipeContextAgent<ConsumerContext<TKey, TValue>> asyncContext, CancellationToken cancellationToken)
        {
            Task<ConsumerContext<TKey, TValue>> Create(ClientContext clientContext, CancellationToken createCancellationToken)
            {
                ConsumerBuilder<TKey, TValue> consumerBuilder = _consumerBuilderFactory();
                ConsumerContext<TKey, TValue> context = new KafkaConsumerContext<TKey, TValue>(_hostConfiguration, _receiveSettings,
                    _headersDeserializer, consumerBuilder, cancellationToken, _checkpointPipeConfiguration);
                return Task.FromResult(context);
            }

            _clientContextSupervisor.CreateAgent(asyncContext, Create, cancellationToken);
        }
    }
}
