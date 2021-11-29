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
        readonly MessageContext _adapter;

        public KafkaConsumeContext(ReceiveContext receiveContext, ConsumeResult<TKey, TValue> result)
            : base(receiveContext)
        {
            Message = result.Message.Value;
            _adapter = new KafkaHeaderAdapter<TKey, TValue>(result, receiveContext);
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

        public TValue Message { get; }

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
    }
}
