namespace MassTransit.Transformation
{
    using System;
    using Middleware;


    /// <summary>
    /// Sits in front of the consume context and allows the inbound message to be
    /// transformed.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public class ConsumeTransformContext<TInput> :
        ProxyPipeContext,
        TransformContext<TInput>
        where TInput : class
    {
        readonly ConsumeContext _context;

        public ConsumeTransformContext(ConsumeContext context, TInput input)
            : base(context)
        {
            _context = context;
            Input = input;
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

        public bool HasInput => true;

        public TInput Input { get; }
    }
}
