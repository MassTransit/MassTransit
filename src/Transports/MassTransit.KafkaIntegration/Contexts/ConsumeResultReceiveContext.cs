namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Context;
    using Serializers;
    using Transports;
    using Util;


    public sealed class ConsumeResultReceiveContext<TKey, TValue> :
        BaseReceiveContext,
        ConsumeResultContext,
        ReceiveLockContext
        where TValue : class
    {
        readonly IHeadersDeserializer _headersDeserializer;
        readonly IConsumerLockContext<TKey, TValue> _lockContext;
        readonly ConsumeResult<TKey, TValue> _result;

        public ConsumeResultReceiveContext(ConsumeResult<TKey, TValue> result, ReceiveEndpointContext receiveEndpointContext,
            IConsumerLockContext<TKey, TValue> lockContext, IHeadersDeserializer headersDeserializer)
            : base(false, receiveEndpointContext)
        {
            _result = result;
            _lockContext = lockContext;
            _headersDeserializer = headersDeserializer;

            var consumeContext = new KafkaConsumeContext<TKey, TValue>(this, _result);

            AddOrUpdatePayload<ConsumeContext>(() => consumeContext, existing => consumeContext);
        }

        protected override IHeaderProvider HeaderProvider => _headersDeserializer.Deserialize(Headers);

        public string Topic => _result.Topic;

        public Partition Partition => _result.Partition;

        public Offset Offset => _result.Offset;

        public Timestamp Timestamp => _result.Message.Timestamp;

        public Headers Headers => _result.Message.Headers;

        public Task Complete()
        {
            return _lockContext.Complete(_result);
        }

        public Task Faulted(Exception exception)
        {
            return TaskUtil.Completed;
        }

        public Task ValidateLockStatus()
        {
            return TaskUtil.Completed;
        }

        public override byte[] GetBody()
        {
            throw new NotSupportedException();
        }

        public override Stream GetBodyStream()
        {
            throw new NotSupportedException();
        }
    }
}
