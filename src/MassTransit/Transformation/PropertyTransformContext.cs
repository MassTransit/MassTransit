namespace MassTransit.Transformation
{
    using System;
    using Metadata;
    using Middleware;


    /// <summary>
    /// For nested types transformed.
    /// </summary>
    /// <typeparam name="TMessage">The input message transform context type</typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class PropertyTransformContext<TMessage, TProperty> :
        ProxyPipeContext,
        TransformContext<TProperty>
        where TMessage : class
        where TProperty : class
    {
        readonly TransformContext<TMessage> _context;

        public PropertyTransformContext(TransformContext<TMessage> context, TProperty property)
            : base(context)
        {
            _context = context;
            Input = property;
        }

        public Guid? MessageId => _context.MessageId;
        public Guid? RequestId => _context.RequestId;
        public Guid? CorrelationId => _context.CorrelationId;
        public Guid? ConversationId => _context.ConversationId;
        public Guid? InitiatorId => _context.InitiatorId;
        public DateTime? ExpirationTime => _context.ExpirationTime;
        public Uri SourceAddress => _context.SourceAddress;
        public Uri DestinationAddress => _context.DestinationAddress;
        public Uri ResponseAddress => _context.ResponseAddress;
        public Uri FaultAddress => _context.FaultAddress;
        public DateTime? SentTime => default;
        public Headers Headers => _context.Headers;
        public HostInfo Host => HostMetadataCache.Host;

        public bool HasInput => true;
        public TProperty Input { get; }
    }
}
