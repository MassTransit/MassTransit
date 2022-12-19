namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Serialization;
    using Transports;


    public sealed class KafkaReceiveContext<TKey, TValue> :
        BaseReceiveContext,
        KafkaConsumeContext<TKey>,
        ReceiveLockContext
        where TValue : class
    {
        readonly ConsumeResult<byte[], byte[]> _result;
        readonly KafkaReceiveEndpointContext<TKey, TValue> _receiveEndpointContext;
        readonly IConsumerLockContext _lockContext;
        readonly Lazy<TKey> _keyLazy;
        IHeaderProvider _headerProvider;

        public KafkaReceiveContext(ConsumeResult<byte[], byte[]> result, KafkaReceiveEndpointContext<TKey, TValue> receiveEndpointContext,
            IConsumerLockContext lockContext)
            : base(false, receiveEndpointContext)
        {
            _result = result;
            _receiveEndpointContext = receiveEndpointContext;
            _lockContext = lockContext;

            Body = new NotSupportedMessageBody();
            InputAddress = receiveEndpointContext.NormalizeAddress(new Uri($"topic:{_result.Topic}"));

            var consumeContext = new KafkaConsumeContext<TKey, TValue>(this, _result, receiveEndpointContext.ValueDeserializer);
            _keyLazy = new Lazy<TKey>(Deserialize);

            AddOrUpdatePayload<ConsumeContext>(() => consumeContext, existing => consumeContext);
        }

        protected override IHeaderProvider HeaderProvider =>
            _headerProvider ??= _receiveEndpointContext.HeadersDeserializer.Deserialize(_result.Message.Headers);

        public override MessageBody Body { get; }

        public TKey Key => _keyLazy.Value;

        public string Topic => _result.Topic;

        public int Partition => _result.Partition;

        public long Offset => _result.Offset;

        public DateTime CheckpointUtcDateTime => _result.Message.Timestamp.UtcDateTime;

        public Task Complete()
        {
            return _lockContext.Complete(_result);
        }

        public Task Faulted(Exception exception)
        {
            return _lockContext.Faulted(_result, exception);
        }

        public Task ValidateLockStatus()
        {
            return Task.CompletedTask;
        }

        TKey Deserialize()
        {
            ReadOnlySpan<byte> span = _result.Message.Key?.Length > 0 ? new ReadOnlySpan<byte>(_result.Message.Key) : ReadOnlySpan<byte>.Empty;
            var context = new SerializationContext(MessageComponentType.Key, _result.Topic, _result.Message.Headers);
            return _receiveEndpointContext.KeyDeserializer.Deserialize(span, span.IsEmpty, context);
        }
    }
}
