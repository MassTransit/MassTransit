namespace MassTransit.KafkaIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Context;


    public class KafkaConsumeContext<TKey, TValue> :
        DeserializerConsumeContext,
        ConsumeContext<TValue>
        where TValue : class
    {
        readonly IDeserializer<TValue> _deserializer;
        readonly MessageContext _adapter;
        readonly ConsumeResult<byte[], byte[]> _result;
        readonly Lazy<TValue> _messageLazy;

        public KafkaConsumeContext(ReceiveContext receiveContext, ConsumeResult<byte[], byte[]> result, IDeserializer<TValue> deserializer)
            : base(receiveContext)
        {
            _deserializer = deserializer;
            _result = result;
            _adapter = new KafkaHeaderAdapter(result, receiveContext);
            _messageLazy = new Lazy<TValue>(Deserialize);
        }

        public override bool HasMessageType(Type messageType)
        {
            return messageType.IsAssignableFrom(typeof(TValue));
        }

        public override bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
        {
            if (Message is T message)
            {
                consumeContext = new MessageConsumeContext<T>(this, message);
                return true;
            }

            consumeContext = default;
            return false;
        }

        public override Guid? MessageId => _adapter.MessageId;
        public override Guid? RequestId => _adapter.RequestId;
        public override Guid? CorrelationId => _adapter.CorrelationId;
        public override Guid? ConversationId => _adapter.ConversationId;
        public override Guid? InitiatorId => _adapter.InitiatorId;
        public override DateTime? ExpirationTime => _adapter.ExpirationTime;
        public override Uri SourceAddress => _adapter.SourceAddress;
        public override Uri DestinationAddress => _adapter.DestinationAddress;
        public override Uri ResponseAddress => _adapter.ResponseAddress;
        public override Uri FaultAddress => _adapter.FaultAddress;
        public override DateTime? SentTime => _adapter.SentTime;
        public override MassTransit.Headers Headers => _adapter.Headers;
        public override HostInfo Host => _adapter.Host;

        public override IEnumerable<string> SupportedMessageTypes => MessageTypeCache<TValue>.MessageTypeNames;

        public TValue Message => _messageLazy.Value;

        public Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return ReceiveContext.NotifyConsumed(this, duration, consumerType);
        }

        public Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return ReceiveContext.NotifyFaulted(this, duration, consumerType, exception);
        }

        protected override Task GenerateFault<T>(ConsumeContext<T> context, Exception exception)
        {
            return Task.CompletedTask;
        }

        TValue Deserialize()
        {
            ReadOnlySpan<byte> span = _result.Message.Value?.Length > 0 ? new ReadOnlySpan<byte>(_result.Message.Value) : ReadOnlySpan<byte>.Empty;
            var context = new SerializationContext(MessageComponentType.Value, _result.Topic, _result.Message.Headers);
            return _deserializer.Deserialize(span, span.IsEmpty, context);
        }
    }
}
