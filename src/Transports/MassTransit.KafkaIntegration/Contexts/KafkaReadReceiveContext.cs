namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using System.IO;
    using Confluent.Kafka;
    using Context;
    using Serializers;


    public sealed class KafkaReadReceiveContext<TKey, TValue> :
        BaseReceiveContext,
        KafkaReadContext
        where TValue : class
    {
        readonly ConsumeResult<TKey, TValue> _consumeResult;
        readonly IHeadersDeserializer _headersDeserializer;

        public KafkaReadReceiveContext(ConsumeResult<TKey, TValue> consumeResult, ReceiveEndpointContext receiveEndpointContext,
            IHeadersDeserializer headersDeserializer)
            : base(false, receiveEndpointContext)
        {
            _consumeResult = consumeResult;
            _headersDeserializer = headersDeserializer;

            var consumeContext = new KafkaConsumeContext<TKey, TValue>(this, _consumeResult);

            AddOrUpdatePayload<ConsumeContext>(() => consumeContext, existing => consumeContext);
        }

        protected override IHeaderProvider HeaderProvider => _headersDeserializer.Deserialize(Headers);

        public string Topic => _consumeResult.Topic;

        public Partition Partition => _consumeResult.Partition;

        public Offset Offset => _consumeResult.Offset;

        public Timestamp Timestamp => _consumeResult.Message.Timestamp;

        public Headers Headers => _consumeResult.Message.Headers;

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
