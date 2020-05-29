namespace MassTransit.Context
{
    using System;
    using Metadata;


    public class MessageSendContextAdapter :
        MessageContext
    {
        readonly SendContext _context;

        public MessageSendContextAdapter(SendContext context)
        {
            _context = context;
        }

        Guid? MessageContext.MessageId => _context.MessageId;

        Guid? MessageContext.RequestId => _context.RequestId;

        Guid? MessageContext.CorrelationId => _context.CorrelationId;

        Guid? MessageContext.ConversationId => _context.ConversationId;

        Guid? MessageContext.InitiatorId => _context.InitiatorId;

        DateTime? MessageContext.ExpirationTime => _context.TimeToLive.HasValue ? DateTime.UtcNow + _context.TimeToLive : default;

        Uri MessageContext.SourceAddress => _context.SourceAddress;

        Uri MessageContext.DestinationAddress => _context.DestinationAddress;

        Uri MessageContext.ResponseAddress => _context.ResponseAddress;

        Uri MessageContext.FaultAddress => _context.FaultAddress;

        DateTime? MessageContext.SentTime => _context.SentTime;

        Headers MessageContext.Headers => _context.Headers;

        HostInfo MessageContext.Host => HostMetadataCache.Host;
    }
}
