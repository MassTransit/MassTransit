namespace MassTransit.Transformation.Contexts
{
    using System;
    using GreenPipes;
    using Metadata;


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
        DateTime? MessageContext.SentTime => default;
        Headers MessageContext.Headers => _context.Headers;
        HostInfo MessageContext.Host => HostMetadataCache.Host;

        public bool HasInput => true;
        public TProperty Input { get; }
    }
}
