namespace MassTransit
{
    using System;


    public abstract class MessageEvent<TMessage> :
        IMessageEvent<TMessage>
        where TMessage : class
    {
        readonly MessageContext _context;
        readonly TMessage _message;

        protected MessageEvent(MessageContext context, TMessage message)
        {
            _context = context;
            _message = message;
        }

        Guid? MessageContext.MessageId => _context.MessageId;
        Guid? MessageContext.RequestId => _context.RequestId;
        Guid? MessageContext.CorrelationId => _context.CorrelationId;
        Guid? MessageContext.ConversationId => _context.ConversationId;
        Guid? MessageContext.InitiatorId => _context.InitiatorId;

        DateTime? MessageContext.ExpirationTime => _context.ExpirationTime;

        Uri MessageContext.SourceAddress => _context.SourceAddress;
        Uri MessageContext.DestinationAddress => _context.DestinationAddress;
        Uri MessageContext.ResponseAddress => _context.ResponseAddress;
        Uri MessageContext.FaultAddress => _context.FaultAddress;

        DateTime? MessageContext.SentTime => _context.SentTime;

        Headers MessageContext.Headers => _context.Headers;

        HostInfo MessageContext.Host => _context.Host;

        Type IMessageEvent.MessageType => typeof(TMessage);
    }
}