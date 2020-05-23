namespace MassTransit.KafkaIntegration.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Context;
    using GreenPipes.Util;
    using Metadata;


    public class KafkaConsumeContext<TKey, TValue> :
        DeserializerConsumeContext,
        ConsumeContext<TValue>
        where TValue : class
    {
        readonly ConsumeResult<TKey, TValue> _consumeResult;

        public KafkaConsumeContext(ReceiveContext receiveContext, ConsumeResult<TKey, TValue> consumeResult)
            : base(receiveContext)
        {
            _consumeResult = consumeResult;
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

        public override Guid? MessageId { get; }

        public override Guid? RequestId { get; }

        public override Guid? CorrelationId { get; }

        public override Guid? ConversationId { get; }

        public override Guid? InitiatorId { get; }

        public override DateTime? ExpirationTime { get; }

        public override Uri SourceAddress { get; }

        public override Uri DestinationAddress { get; }

        public override Uri ResponseAddress { get; }

        public override Uri FaultAddress { get; }

        public override DateTime? SentTime { get; }

        public override MassTransit.Headers Headers { get; }

        public override HostInfo Host => HostMetadataCache.Host;

        public override IEnumerable<string> SupportedMessageTypes => TypeMetadataCache<TValue>.MessageTypeNames;

        public TValue Message => _consumeResult.Message.Value;

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
            return TaskUtil.Completed;
        }
    }
}
