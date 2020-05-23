namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using System.IO;
    using Confluent.Kafka;
    using Context;
    using Transports;


    public sealed class KafkaReadReceiveContext<TKey, TValue> :
        BaseReceiveContext,
        KafkaReadContext
        where TValue : class
    {
        readonly KafkaConsumeContext<TKey, TValue> _consumeContext;
        readonly ConsumeResult<TKey, TValue> _consumeResult;

        public KafkaReadReceiveContext(ConsumeResult<TKey, TValue> consumeResult, ReceiveEndpointContext receiveEndpointContext)
            : base(false, receiveEndpointContext)
        {
            _consumeResult = consumeResult;

            _consumeContext = new KafkaConsumeContext<TKey, TValue>(this, _consumeResult);

            AddOrUpdatePayload<ConsumeContext>(() => _consumeContext, existing => _consumeContext);
        }

        protected override IHeaderProvider HeaderProvider => new DictionaryHeaderProvider(_consumeResult.Properties);

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
