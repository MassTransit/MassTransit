namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Context;
    using GreenPipes;
    using Serializers;
    using Transport;
    using Util;


    public class KafkaConsumerContext<TKey, TValue> :
        BasePipeContext,
        IKafkaConsumerContext<TKey, TValue>
        where TValue : class
    {
        readonly IConsumer<TKey, TValue> _consumer;
        readonly ChannelExecutor _executor;
        readonly IConsumerLockContext<TKey, TValue> _lockContext;

        public KafkaConsumerContext(ILogContext logContext, ReceiveSettings receiveSettings, IHeadersDeserializer headersDeserializer,
            ConsumerBuilder<TKey, TValue> consumerBuilder, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            var lockContext = new ConsumerLockContext<TKey, TValue>(logContext, receiveSettings);

            _consumer = consumerBuilder
                .SetPartitionsAssignedHandler(lockContext.OnAssigned)
                .SetPartitionsRevokedHandler(lockContext.OnUnAssigned)
                .SetErrorHandler(OnError)
                .Build();
            ReceiveSettings = receiveSettings;
            HeadersDeserializer = headersDeserializer;

            _lockContext = lockContext;
            _executor = new ChannelExecutor(1);
        }

        public event Action<IConsumer<TKey, TValue>, Error> ErrorHandler;

        public ReceiveSettings ReceiveSettings { get; }

        public IHeadersDeserializer HeadersDeserializer { get; }

        public Task Subscribe()
        {
            _consumer.Subscribe(ReceiveSettings.Topic);
            return TaskUtil.Completed;
        }

        public Task Close()
        {
            _consumer.Close();
            return TaskUtil.Completed;
        }

        public Task<ConsumeResult<TKey, TValue>> Consume(CancellationToken cancellationToken)
        {
            return _executor.Run(() => _consumer.Consume(cancellationToken), cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            await _executor.DisposeAsync().ConfigureAwait(false);
            _consumer.Dispose();
        }

        public Task Complete(ConsumeResult<TKey, TValue> result)
        {
            return _lockContext.Complete(result);
        }

        void OnError(IConsumer<TKey, TValue> consumer, Error error)
        {
            ErrorHandler?.Invoke(consumer, error);
        }
    }
}
