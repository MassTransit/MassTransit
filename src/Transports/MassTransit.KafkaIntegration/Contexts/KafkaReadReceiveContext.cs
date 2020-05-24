namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Confluent.Kafka;
    using Context;
    using Transports;


    public sealed class KafkaReadReceiveContext<TKey, TValue> :
        BaseReceiveContext,
        KafkaReadContext
        where TValue : class
    {
        readonly ConsumeResult<TKey, TValue> _consumeResult;

        public KafkaReadReceiveContext(ConsumeResult<TKey, TValue> consumeResult, ReceiveEndpointContext receiveEndpointContext)
            : base(false, receiveEndpointContext)
        {
            _consumeResult = consumeResult;

            var consumeContext = new KafkaConsumeContext<TKey, TValue>(this, _consumeResult);

            AddOrUpdatePayload<ConsumeContext>(() => consumeContext, existing => consumeContext);
        }

        protected override IHeaderProvider HeaderProvider =>
            new DictionaryHeaderProvider(_consumeResult.Message.Headers.ToDictionary(x => x.Key, x => (object)Encoding.UTF8.GetString(x.GetValueBytes())));

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
