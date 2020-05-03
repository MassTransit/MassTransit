namespace MassTransit.Mediator.Contexts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;
    using Metadata;
    using Util;


    public class MediatorConsumeContext<TMessage> :
        DeserializerConsumeContext,
        ConsumeContext<TMessage>
        where TMessage : class
    {
        readonly SendContext<TMessage> _sendContext;

        public MediatorConsumeContext(ReceiveContext receiveContext, SendContext<TMessage> sendContext)
            : base(receiveContext)
        {
            Message = sendContext.Message;
            _sendContext = sendContext;
        }

        public override IEnumerable<string> SupportedMessageTypes => TypeMetadataCache<TMessage>.MessageTypeNames;

        public override bool HasMessageType(Type messageType)
        {
            return messageType.IsAssignableFrom(typeof(TMessage));
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

        public override Guid? MessageId => _sendContext.MessageId;

        public override Guid? RequestId => _sendContext.RequestId;

        public override Guid? CorrelationId => _sendContext.CorrelationId;

        public override Guid? ConversationId => _sendContext.ConversationId;

        public override Guid? InitiatorId => _sendContext.InitiatorId;

        public override DateTime? ExpirationTime => _sendContext.SentTime + _sendContext.TimeToLive;

        public override Uri SourceAddress => _sendContext.SourceAddress;

        public override Uri DestinationAddress => _sendContext.DestinationAddress;

        public override Uri ResponseAddress => _sendContext.ResponseAddress;

        public override Uri FaultAddress => _sendContext.FaultAddress;

        public override DateTime? SentTime => _sendContext.SentTime;

        public override Headers Headers => _sendContext.Headers;

        public override HostInfo Host => HostMetadataCache.Host;

        public TMessage Message { get; }

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
