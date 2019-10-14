namespace MassTransit.Transformation.Contexts
{
    using System;
    using GreenPipes;
    using Metadata;


    /// <summary>
    /// Sits in front of the consume context and allows the inbound message to be
    /// transformed.
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class SendTransformContext<TMessage> :
        ProxyPipeContext,
        TransformContext<TMessage>
        where TMessage : class
    {
        readonly SendContext<TMessage> _context;

        public SendTransformContext(SendContext<TMessage> context)
            : base(context)
        {
            _context = context;
        }

        Guid? MessageContext.MessageId => _context.MessageId;
        Guid? MessageContext.RequestId => _context.RequestId;
        Guid? MessageContext.CorrelationId => _context.CorrelationId;
        Guid? MessageContext.ConversationId => _context.ConversationId;
        Guid? MessageContext.InitiatorId => _context.InitiatorId;
        DateTime? MessageContext.ExpirationTime => DateTime.UtcNow + _context.TimeToLive;
        Uri MessageContext.SourceAddress => _context.SourceAddress;
        Uri MessageContext.DestinationAddress => _context.DestinationAddress;
        Uri MessageContext.ResponseAddress => _context.ResponseAddress;
        Uri MessageContext.FaultAddress => _context.FaultAddress;
        DateTime? MessageContext.SentTime => default;
        Headers MessageContext.Headers => _context.Headers;
        HostInfo MessageContext.Host => HostMetadataCache.Host;

        public bool HasInput => true;

        public TMessage Input => _context.Message;
    }
}
