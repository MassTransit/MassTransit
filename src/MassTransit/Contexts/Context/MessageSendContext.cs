namespace MassTransit.Context
{
    using System;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Threading;
    using Middleware;
    using Serialization;


    public class MessageSendContext<TMessage> :
        BasePipeContext,
        PublishContext<TMessage>
        where TMessage : class
    {
        readonly Lazy<MessageBody> _body;
        readonly DictionarySendHeaders _headers;

        IMessageSerializer _serializer;

        public MessageSendContext(TMessage message, CancellationToken cancellationToken = default)
            : base(cancellationToken)
        {
            Message = message;

            _headers = new DictionarySendHeaders();

            var messageId = NewId.Next();

            MessageId = messageId.ToGuid();
            SentTime = messageId.Timestamp;

            _body = new Lazy<MessageBody>(() => GetMessageBody());
        }

        /// <summary>
        /// Set to true if the message is being published
        /// </summary>
        public bool IsPublish { get; set; }

        public MessageBody Body => _body.Value;

        public virtual TimeSpan? Delay { get; set; }

        public Guid? MessageId { get; set; }
        public Guid? RequestId { get; set; }
        public Guid? CorrelationId { get; set; }

        public Guid? ConversationId { get; set; }
        public Guid? InitiatorId { get; set; }

        public Guid? ScheduledMessageId { get; set; }

        public SendHeaders Headers => _headers;

        public Uri SourceAddress { get; set; }
        public Uri DestinationAddress { get; set; }
        public Uri ResponseAddress { get; set; }
        public Uri FaultAddress { get; set; }

        public TimeSpan? TimeToLive { get; set; }
        public DateTime? SentTime { get; private set; }

        public ContentType ContentType { get; set; }

        public IMessageSerializer Serializer
        {
            get => _serializer;
            set
            {
                if (_body.IsValueCreated)
                    throw new InvalidOperationException("The message was already serialized");

                _serializer = value;
                if (_serializer != null)
                    ContentType = _serializer.ContentType;
            }
        }

        public ISerialization Serialization { get; set; }

        public long? BodyLength => _body.IsValueCreated ? _body.Value.Length : default;

        public SendContext<T> CreateProxy<T>(T message)
            where T : class
        {
            return new SendContextProxy<T>(this, message);
        }

        public bool Durable { get; set; } = true;

        public TMessage Message { get; }

        public bool Mandatory { get; set; }

        MessageBody GetMessageBody()
        {
            return Serializer?.GetMessageBody(this) ?? throw new SerializationException("Unable to serialize message, no serializer specified.");
        }
    }
}
