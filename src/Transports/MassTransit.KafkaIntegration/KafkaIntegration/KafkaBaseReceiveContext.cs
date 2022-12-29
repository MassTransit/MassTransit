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
        readonly KafkaReceiveEndpointContext<TKey, TValue> _context;
        readonly Lazy<TKey> _key;
        readonly IConsumerLockContext _lockContext;
        readonly ConsumeResult<byte[], byte[]> _result;
        IHeaderProvider _headerProvider;

        public KafkaReceiveContext(ConsumeResult<byte[], byte[]> result, KafkaReceiveEndpointContext<TKey, TValue> context, IConsumerLockContext lockContext)
            : base(false, context)
        {
            _result = result;
            _context = context;
            _lockContext = lockContext;

            InputAddress = context.NormalizeAddress(new Uri($"topic:{_result.Topic}"));

            _key = new Lazy<TKey>(() => result.Message.DeserializeKey(result.Topic, _context.KeyDeserializer));

            var messageContext = new KafkaMessageContext(_result, this);

            var serializerContext = new KafkaSerializationContext<TValue>(_result, context.ValueDeserializer, messageContext);

            var consumeContext = new KafkaConsumeContext<TKey, TValue>(this, serializerContext);

            AddOrUpdatePayload<ConsumeContext>(() => consumeContext, existing => consumeContext);
        }

        protected override IHeaderProvider HeaderProvider => _headerProvider ??= _context.HeadersDeserializer.Deserialize(_result.Message.Headers);

        public override MessageBody Body => new NotSupportedMessageBody();

        public string GroupId => _context.GroupId;

        public TKey Key => _key.Value;

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
    }
}
