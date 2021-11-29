#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public abstract class BaseSerializerContext :
        SerializerContext
    {
        readonly MessageContext _context;
        readonly IObjectDeserializer _deserializer;

        Guid? _conversationId;
        Guid? _correlationId;
        Uri? _destinationAddress;
        Uri? _faultAddress;
        Headers? _headers;
        Guid? _initiatorId;
        Guid? _messageId;
        Guid? _requestId;
        Uri? _responseAddress;
        Uri? _sourceAddress;

        protected BaseSerializerContext(IObjectDeserializer deserializer, MessageContext context, string[] supportedMessageTypes)
        {
            _context = context;
            SupportedMessageTypes = supportedMessageTypes;
            _deserializer = deserializer;
        }

        public Guid? MessageId => _messageId ??= _context.MessageId;
        public Guid? RequestId => _requestId ??= _context.RequestId;
        public Guid? CorrelationId => _correlationId ??= _context.CorrelationId;
        public Guid? ConversationId => _conversationId ??= _context.ConversationId;
        public Guid? InitiatorId => _initiatorId ??= _context.InitiatorId;
        public DateTime? ExpirationTime => _context.ExpirationTime;
        public Uri? SourceAddress => _sourceAddress ??= _context.SourceAddress;
        public Uri? DestinationAddress => _destinationAddress ??= _context.DestinationAddress;
        public Uri? ResponseAddress => _responseAddress ??= _context.ResponseAddress;
        public Uri? FaultAddress => _faultAddress ??= _context.FaultAddress;
        public DateTime? SentTime => _context.SentTime;
        public Headers Headers => _headers ??= _context.Headers;
        public HostInfo Host => _context.Host;

        public string[] SupportedMessageTypes { get; }

        public T? DeserializeObject<T>(object? value, T? defaultValue = default)
            where T : class
        {
            return _deserializer.DeserializeObject(value, defaultValue);
        }

        public T? DeserializeObject<T>(object? value, T? defaultValue = null)
            where T : struct
        {
            return _deserializer.DeserializeObject(value, defaultValue);
        }

        public abstract bool TryGetMessage<T>(out T? message)
            where T : class;

        public abstract bool TryGetMessage(Type messageType, out object? message);

        public abstract IMessageSerializer GetMessageSerializer();

        public abstract IMessageSerializer GetMessageSerializer<T>(MessageEnvelope envelope, T message)
            where T : class;

        public abstract IMessageSerializer GetMessageSerializer(object message, string[] messageTypes);

        public abstract Dictionary<string, object> ToDictionary<T>(T? message)
            where T : class;

        public virtual bool IsSupportedMessageType<T>()
            where T : class
        {
            var typeUrn = MessageUrn.ForTypeString<T>();

            return SupportedMessageTypes.Any(x => typeUrn.Equals(x, StringComparison.OrdinalIgnoreCase));
        }
    }
}
