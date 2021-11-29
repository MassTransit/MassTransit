namespace MassTransit.Transformation
{
    using System;
    using Metadata;
    using Middleware;


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

        public Guid? MessageId => _context.MessageId;
        public Guid? RequestId => _context.RequestId;
        public Guid? CorrelationId => _context.CorrelationId;
        public Guid? ConversationId => _context.ConversationId;
        public Guid? InitiatorId => _context.InitiatorId;
        public DateTime? ExpirationTime => DateTime.UtcNow + _context.TimeToLive;
        public Uri SourceAddress => _context.SourceAddress;
        public Uri DestinationAddress => _context.DestinationAddress;
        public Uri ResponseAddress => _context.ResponseAddress;
        public Uri FaultAddress => _context.FaultAddress;
        public DateTime? SentTime => default;
        public Headers Headers => _context.Headers;
        public HostInfo Host => HostMetadataCache.Host;

        public bool HasInput => true;

        public TMessage Input => _context.Message;
    }
}
