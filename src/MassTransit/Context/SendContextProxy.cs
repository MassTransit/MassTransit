namespace MassTransit.Context
{
    using System;
    using System.Net.Mime;
    using GreenPipes;


    public class SendContextProxy<TMessage> :
        SendContextProxy,
        PublishContext<TMessage>
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
        BasePipeContext,
        PublishContext
    {
        readonly SendContext _context;

        protected SendContextProxy(SendContext context)
            : base(context)
        {
            _context = context;
        }

        public override bool HasPayloadType(Type payloadType)
        {
            if (base.HasPayloadType(payloadType))
                return true;

            return _context.HasPayloadType(payloadType);
        }

        public override bool TryGetPayload<T>(out T payload)
        {
            if (base.TryGetPayload(out payload))
                return true;

            return _context.TryGetPayload(out payload);
        }

        public override T GetOrAddPayload<T>(PayloadFactory<T> payloadFactory)
        {
            if (base.TryGetPayload<T>(out var existing))
                return existing;

            if (_context.TryGetPayload(out existing))
                return existing;

            return base.GetOrAddPayload(payloadFactory);
        }

        public override T AddOrUpdatePayload<T>(PayloadFactory<T> addFactory, UpdatePayloadFactory<T> updateFactory)
        {
            if (base.TryGetPayload<T>(out var existing) || _context.TryGetPayload(out existing))
            {
                T Update(T _)
                {
                    return updateFactory(existing);
                }

                T Add()
                {
                    return updateFactory(existing);
                }

                return base.AddOrUpdatePayload(Add, Update);
            }

            return base.AddOrUpdatePayload(addFactory, updateFactory);
        }

        public Uri SourceAddress
        {
            get => _context.SourceAddress;
            set => _context.SourceAddress = value;
        }

        public Uri DestinationAddress
        {
            get => _context.DestinationAddress;
            set => _context.DestinationAddress = value;
        }

        public Uri ResponseAddress
        {
            get => _context.ResponseAddress;
            set => _context.ResponseAddress = value;
        }

        public Uri FaultAddress
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

        public ContentType ContentType
        {
            get => _context.ContentType;
            set => _context.ContentType = value;
        }

        public bool Durable
        {
            get => _context.Durable;
            set => _context.Durable = value;
        }

        public IMessageSerializer Serializer
        {
            get => _context.Serializer;
            set => _context.Serializer = value;
        }

        public SendContext<T> CreateProxy<T>(T message)
            where T : class
        {
            return _context.CreateProxy(message);
        }

        public bool Mandatory
        {
            get => _context.GetPayload<PublishContext>().Mandatory;
            set => _context.GetPayload<PublishContext>().Mandatory = value;
        }
    }
}
