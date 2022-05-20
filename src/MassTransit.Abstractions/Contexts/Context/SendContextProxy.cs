namespace MassTransit.Context
{
    using System;
    using System.Net.Mime;
    using Middleware;


    public class SendContextProxy<TMessage> :
        SendContextProxy,
        SendContext<TMessage>
        where TMessage : class
    {
        public SendContextProxy(SendContext context, TMessage message)
            : base(context)
        {
            Message = message;
        }

        public TMessage Message { get; }
    }


    public class SendContextProxy :
        ProxyPipeContext
    {
        readonly SendContext _context;

        protected SendContextProxy(SendContext context)
            : base(context)
        {
            _context = context;
        }

        public Uri? SourceAddress
        {
            get => _context.SourceAddress;
            set => _context.SourceAddress = value;
        }

        public Uri? DestinationAddress
        {
            get => _context.DestinationAddress;
            set => _context.DestinationAddress = value;
        }

        public Uri? ResponseAddress
        {
            get => _context.ResponseAddress;
            set => _context.ResponseAddress = value;
        }

        public Uri? FaultAddress
        {
            get => _context.FaultAddress;
            set => _context.FaultAddress = value;
        }

        public Guid? RequestId
        {
            get => _context.RequestId;
            set => _context.RequestId = value;
        }

        public Guid? MessageId
        {
            get => _context.MessageId;
            set => _context.MessageId = value;
        }

        public Guid? CorrelationId
        {
            get => _context.CorrelationId;
            set => _context.CorrelationId = value;
        }

        public Guid? ConversationId
        {
            get => _context.ConversationId;
            set => _context.ConversationId = value;
        }

        public Guid? InitiatorId
        {
            get => _context.InitiatorId;
            set => _context.InitiatorId = value;
        }

        public Guid? ScheduledMessageId
        {
            get => _context.ScheduledMessageId;
            set => _context.ScheduledMessageId = value;
        }

        public SendHeaders Headers => _context.Headers;

        public TimeSpan? TimeToLive
        {
            get => _context.TimeToLive;
            set => _context.TimeToLive = value;
        }

        public DateTime? SentTime => _context.SentTime;

        public ContentType? ContentType
        {
            get => _context.ContentType;
            set => _context.ContentType = value;
        }

        public bool Durable
        {
            get => _context.Durable;
            set => _context.Durable = value;
        }

        public TimeSpan? Delay
        {
            get => _context.Delay;
            set => _context.Delay = value;
        }

        public IMessageSerializer Serializer
        {
            get => _context.Serializer;
            set => _context.Serializer = value;
        }

        public ISerialization Serialization
        {
            get => _context.Serialization;
            set => _context.Serialization = value;
        }

        public long? BodyLength => _context.BodyLength;

        public SendContext<T> CreateProxy<T>(T message)
            where T : class
        {
            return _context.CreateProxy(message);
        }
    }
}
