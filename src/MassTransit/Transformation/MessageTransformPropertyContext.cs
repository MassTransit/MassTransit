namespace MassTransit.Transformation
{
    using System;
    using Middleware;


    public class MessageTransformPropertyContext<TProperty, TInput> :
        ProxyPipeContext,
        TransformPropertyContext<TProperty, TInput>
        where TInput : class
    {
        readonly TransformContext<TInput> _context;

        public MessageTransformPropertyContext(TransformContext<TInput> context, TProperty value)
            : base(context)
        {
            _context = context;

            Value = value;
            HasValue = true;
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
        public DateTime? SentTime => _context.SentTime;
        public Headers Headers => _context.Headers;
        public HostInfo Host => _context.Host;

        public bool HasInput => _context.HasInput;
        public TInput Input => _context.Input;

        public bool HasValue { get; }
        public TProperty Value { get; }
    }
}
