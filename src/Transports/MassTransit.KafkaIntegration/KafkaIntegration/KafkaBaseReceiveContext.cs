namespace MassTransit.KafkaIntegration
{
    using System;
    using Confluent.Kafka;
    using Transports;


    public sealed class KafkaReceiveContext<TKey, TValue> :
        BaseReceiveContext,
        KafkaConsumeContext<TKey>
        where TValue : class
    {
        readonly KafkaReceiveEndpointContext<TKey, TValue> _context;
        readonly Lazy<IHeaderProvider> _headerProvider;
        readonly Lazy<TKey> _key;
        readonly ConsumeResult<byte[], byte[]> _result;

        public KafkaReceiveContext(ConsumeResult<byte[], byte[]> result, KafkaReceiveEndpointContext<TKey, TValue> context)
            : base(false, context)
        {
            _result = result;
            _context = context;

            Body = new BytesMessageBody(_result.Message.Value);
            InputAddress = context.GetInputAddress(_result.Topic);

            _key = new Lazy<TKey>(() => _context.KeyDeserializer.DeserializeKey(result));
            _headerProvider = new Lazy<IHeaderProvider>(() =>
                new KafkaHeaderProvider(_result.Message, _context.HeadersDeserializer.Deserialize(_result.Message.Headers)));

            var messageContext = new KafkaMessageContext(_result, this);

            var serializerContext = new KafkaSerializationContext<TValue>(_result, context.ValueDeserializer, messageContext);

            var consumeContext = new KafkaConsumeContext<TKey, TValue>(this, serializerContext);

            AddOrUpdatePayload<ConsumeContext>(() => consumeContext, existing => consumeContext);
        }

        protected override IHeaderProvider HeaderProvider => _headerProvider.Value;

        public override MessageBody Body { get; }

        public string GroupId => _context.GroupId;

        public TKey Key => _key.Value;

        public string Topic => _result.Topic;

        public int Partition => _result.Partition;

        public long Offset => _result.Offset;
        public bool IsPartitionEof => _result.IsPartitionEOF;

        public DateTime CheckpointUtcDateTime => _result.Message.Timestamp.UtcDateTime;
    }
}
