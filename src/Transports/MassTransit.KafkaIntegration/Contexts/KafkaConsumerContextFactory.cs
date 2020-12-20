namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using GreenPipes;
    using GreenPipes.Agents;
    using GreenPipes.Internals.Extensions;
    using Serializers;
    using Transport;


    public class KafkaConsumerContextFactory<TKey, TValue> :
        IPipeContextFactory<IKafkaConsumerContext<TKey, TValue>>
        where TValue : class
    {
        readonly ReceiveSettings _receiveSettings;
        readonly IHeadersDeserializer _headersDeserializer;
        readonly Func<ConsumerBuilder<TKey, TValue>> _consumerBuilderFactory;

        public KafkaConsumerContextFactory(ReceiveSettings receiveSettings, IHeadersDeserializer headersDeserializer,
            Func<ConsumerBuilder<TKey, TValue>> consumerBuilderFactory)
        {
            _receiveSettings = receiveSettings;
            _headersDeserializer = headersDeserializer;
            _consumerBuilderFactory = consumerBuilderFactory;
        }

        public IPipeContextAgent<IKafkaConsumerContext<TKey, TValue>> CreateContext(ISupervisor supervisor)
        {
            IKafkaConsumerContext<TKey, TValue> context = CreateConsumer(supervisor);
            IPipeContextAgent<IKafkaConsumerContext<TKey, TValue>> contextHandle = supervisor.AddContext(Task.FromResult(context));

            return contextHandle;
        }

        public IActivePipeContextAgent<IKafkaConsumerContext<TKey, TValue>> CreateActiveContext(ISupervisor supervisor,
            PipeContextHandle<IKafkaConsumerContext<TKey, TValue>> context, CancellationToken cancellationToken = new CancellationToken())
        {
            return supervisor.AddActiveContext(context, CreateSharedConnection(context.Context, cancellationToken));
        }

        static async Task<IKafkaConsumerContext<TKey, TValue>> CreateSharedConnection(Task<IKafkaConsumerContext<TKey, TValue>> context,
            CancellationToken cancellationToken)
        {
            return context.IsCompletedSuccessfully()
                ? new SharedKafkaConsumerContext<TKey, TValue>(context.Result, cancellationToken)
                : new SharedKafkaConsumerContext<TKey, TValue>(await context.OrCanceled(cancellationToken).ConfigureAwait(false), cancellationToken);
        }

        IKafkaConsumerContext<TKey, TValue> CreateConsumer(ISupervisor supervisor)
        {
            if (supervisor.Stopping.IsCancellationRequested)
                throw new OperationCanceledException($"The connection is stopping and cannot be used: {_receiveSettings.Topic}");

            return new KafkaConsumerContext<TKey, TValue>(_receiveSettings, _headersDeserializer, _consumerBuilderFactory, supervisor.Stopping);
        }
    }
}
