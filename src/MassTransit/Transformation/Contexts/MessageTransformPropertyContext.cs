namespace MassTransit.Transformation.Contexts
{
    using System;
    using GreenPipes;


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

        public bool HasInput => _context.HasInput;
        public TInput Input => _context.Input;

        public bool HasValue { get; }
        public TProperty Value { get; }
    }
}
