namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Serialization;
    using Serializers;
    using Transports;


    public sealed class KafkaReceiveContext<TKey, TValue> :
        BaseReceiveContext,
        KafkaConsumeContext<TKey>,
        ReceiveLockContext
        where TValue : class
    {
        readonly IHeadersDeserializer _headersDeserializer;
        readonly IConsumerLockContext<TKey, TValue> _lockContext;
        readonly ConsumeResult<TKey, TValue> _result;
        IHeaderProvider _headerProvider;

        public KafkaReceiveContext(ConsumeResult<TKey, TValue> result, ReceiveEndpointContext receiveEndpointContext,
            IConsumerLockContext<TKey, TValue> lockContext, IHeadersDeserializer headersDeserializer)
            : base(false, receiveEndpointContext)
        {
            _result = result;
            _lockContext = lockContext;
            _headersDeserializer = headersDeserializer;

            Body = new NotSupportedMessageBody();

            var consumeContext = new KafkaConsumeContext<TKey, TValue>(this, _result);

            AddOrUpdatePayload<ConsumeContext>(() => consumeContext, existing => consumeContext);
        }

        protected override IHeaderProvider HeaderProvider => _headerProvider ??= _headersDeserializer.Deserialize(_result.Message.Headers);

        public override MessageBody Body { get; }

        public TKey Key => _result.Message.Key;

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
