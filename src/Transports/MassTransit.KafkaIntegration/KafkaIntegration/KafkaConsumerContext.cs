namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using MassTransit.Configuration;
    using MassTransit.Middleware;
    using Serializers;
    using Util;


    public class KafkaConsumerContext<TKey, TValue> :
        BasePipeContext,
        ConsumerContext<TKey, TValue>
        where TValue : class
    {
        readonly IConsumer<TKey, TValue> _consumer;
        readonly IConsumerLockContext<TKey, TValue> _lockContext;
        ChannelExecutor _executor;

        public KafkaConsumerContext(IHostConfiguration hostConfiguration, ReceiveSettings receiveSettings, IHeadersDeserializer headersDeserializer,
            ConsumerBuilder<TKey, TValue> consumerBuilder, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            var lockContext = new ConsumerLockContext<TKey, TValue>(hostConfiguration, receiveSettings);

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
            return Task.CompletedTask;
        }

        public async Task Close()
        {
            await _executor.DisposeAsync().ConfigureAwait(false);
            _executor = null;

            _consumer.Close();
        }

        public async Task<ConsumeResult<TKey, TValue>> Consume(CancellationToken cancellationToken)
        {
            if (_executor == null)
                throw new InvalidOperationException("The consumer is being closed");

            ConsumeResult<TKey, TValue> result = await _executor.Run(() => _consumer.Consume(cancellationToken), cancellationToken).ConfigureAwait(false);
            await _lockContext.Pending(result).ConfigureAwait(false);
            return result;
        }

        public async ValueTask DisposeAsync()
        {
            if (_executor != null)
                await _executor.DisposeAsync().ConfigureAwait(false);

            _consumer.Dispose();
        }

        public Task Pending(ConsumeResult<TKey, TValue> result)
        {
            return _lockContext.Pending(result);
        }

        public Task Complete(ConsumeResult<TKey, TValue> result)
        {
            return _lockContext.Complete(result);
        }

        public Task Faulted(ConsumeResult<TKey, TValue> result, Exception exception)
        {
            return _lockContext.Faulted(result, exception);
        }

        void OnError(IConsumer<TKey, TValue> consumer, Error error)
        {
            ErrorHandler?.Invoke(consumer, error);
        }
    }
}
