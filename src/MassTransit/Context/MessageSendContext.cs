namespace MassTransit.Context
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Threading;
    using GreenPipes;


    public class MessageSendContext<TMessage> :
        BasePipeContext,
        PublishContext<TMessage>
        where TMessage : class
    {
        readonly DictionarySendHeaders _headers;
        byte[] _body;
        IMessageSerializer _serializer;

        public MessageSendContext(TMessage message, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            Message = message;

            _headers = new DictionarySendHeaders();

            var messageId = NewId.Next();

            MessageId = messageId.ToGuid();

            SentTime = messageId.Timestamp;

            Durable = true;
        }

        public MessageSendContext(TMessage message)
        {
            Message = message;

            _headers = new DictionarySendHeaders();

            var messageId = NewId.Next();

            MessageId = messageId.ToGuid();

            SentTime = messageId.Timestamp;

            Durable = true;
        }

        public byte[] Body
        {
            get
            {
                if (_body != null)
                    return _body;

                if (Serializer == null)
                    throw new SerializationException("No serializer specified");
                if (Message == null)
                    throw new SendException(typeof(TMessage), DestinationAddress, "No message specified");

                using (var memoryStream = new MemoryStream(8192))
                {
                    Serializer.Serialize(memoryStream, this);

                    _body = memoryStream.ToArray();
                    return _body;
                }
            }
        }

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
        public DateTime? SentTime { get; set; }

        public ContentType ContentType { get; set; }

        public IMessageSerializer Serializer
        {
            get => _serializer;
            set
            {
                _serializer = value;
                if (_serializer != null)
                    ContentType = _serializer.ContentType;
            }
        }

        SendContext<T> SendContext.CreateProxy<T>(T message)
        {
            return new SendContextProxy<T>(this, message);
        }

        public bool Durable { get; set; }

        public TMessage Message { get; }

        public Stream GetBodyStream()
        {
            return new MemoryStream(Body, false);
        }

        public bool Mandatory { get; set; }
    }
}
