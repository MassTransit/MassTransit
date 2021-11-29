namespace MassTransit.Context
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public class MediatorConsumeContext<TMessage> :
        DeserializerConsumeContext,
        ConsumeContext<TMessage>
        where TMessage : class
    {
        public MediatorConsumeContext(ReceiveContext receiveContext, SerializerContext serializerContext, TMessage message)
            : base(receiveContext, serializerContext)
        {
            Message = message;
        }

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

        public override Guid? MessageId => SerializerContext.MessageId;
        public override Guid? RequestId => SerializerContext.RequestId;
        public override Guid? CorrelationId => SerializerContext.CorrelationId;
        public override Guid? ConversationId => SerializerContext.ConversationId;
        public override Guid? InitiatorId => SerializerContext.InitiatorId;
        public override DateTime? ExpirationTime => SerializerContext.ExpirationTime;
        public override Uri SourceAddress => SerializerContext.SourceAddress;
        public override Uri DestinationAddress => SerializerContext.DestinationAddress;
        public override Uri ResponseAddress => SerializerContext.ResponseAddress;
        public override Uri FaultAddress => SerializerContext.FaultAddress;
        public override DateTime? SentTime => SerializerContext.SentTime;
        public override Headers Headers => SerializerContext.Headers;
        public override HostInfo Host => SerializerContext.Host;
        public override IEnumerable<string> SupportedMessageTypes => SerializerContext.SupportedMessageTypes;

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
            return Task.CompletedTask;
        }
    }
}
