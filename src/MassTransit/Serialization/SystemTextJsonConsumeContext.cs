namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.Json;
    using Context;
    using Internals.Extensions;
    using Metadata;


    public class SystemTextJsonConsumeContext :
        DeserializerConsumeContext
    {
        readonly MessageEnvelope _envelope;
        readonly object _locker = new object();
        readonly IDictionary<Type, ConsumeContext> _messageTypes;
        readonly string[] _supportedTypes;

        Guid? _conversationId;
        Guid? _correlationId;
        Uri _destinationAddress;
        Uri _faultAddress;
        Headers _headers;
        Guid? _initiatorId;
        Guid? _messageId;
        Guid? _requestId;
        Uri _responseAddress;
        Uri _sourceAddress;

        public SystemTextJsonConsumeContext(ReceiveContext receiveContext, MessageEnvelope envelope)
            : base(receiveContext)
        {
            _envelope = envelope ?? throw new ArgumentNullException(nameof(envelope));
            _supportedTypes = envelope.MessageType?.ToArray() ?? Array.Empty<string>();
            _messageTypes = new Dictionary<Type, ConsumeContext>();
        }

        public override Guid? MessageId => _messageId ??= ConvertIdToGuid(_envelope.MessageId);
        public override Guid? RequestId => _requestId ??= ConvertIdToGuid(_envelope.RequestId);
        public override Guid? CorrelationId => _correlationId ??= ConvertIdToGuid(_envelope.CorrelationId);
        public override Guid? ConversationId => _conversationId ??= ConvertIdToGuid(_envelope.ConversationId);
        public override Guid? InitiatorId => _initiatorId ??= ConvertIdToGuid(_envelope.InitiatorId);
        public override DateTime? ExpirationTime => _envelope.ExpirationTime;
        public override Uri SourceAddress => _sourceAddress ??= ConvertToUri(_envelope.SourceAddress);
        public override Uri DestinationAddress => _destinationAddress ??= ConvertToUri(_envelope.DestinationAddress);
        public override Uri ResponseAddress => _responseAddress ??= ConvertToUri(_envelope.ResponseAddress);
        public override Uri FaultAddress => _faultAddress ??= ConvertToUri(_envelope.FaultAddress);
        public override DateTime? SentTime => _envelope.SentTime;

        public override Headers Headers =>
            _headers ??= _envelope.Headers != null ? (Headers)new JsonEnvelopeHeaders(_envelope.Headers) : NoMessageHeaders.Instance;

        public override HostInfo Host => _envelope.Host;
        public override IEnumerable<string> SupportedMessageTypes => _supportedTypes;

        public override bool HasMessageType(Type messageType)
        {
            lock (_locker)
            {
                if (_messageTypes.TryGetValue(messageType, out var existing))
                    return existing != null;
            }

            var typeUrn = MessageUrn.ForTypeString(messageType);

            return _supportedTypes.Any(x => typeUrn.Equals(x, StringComparison.OrdinalIgnoreCase));
        }

        public override bool TryGetMessage<T>(out ConsumeContext<T> message)
        {
            lock (_locker)
            {
                if (_messageTypes.TryGetValue(typeof(T), out var existing))
                {
                    message = existing as ConsumeContext<T>;
                    return message != null;
                }

                var messageToken = GetMessageToken(_envelope.Message);

                if (typeof(T) == typeof(JsonElement))
                {
                    _messageTypes[typeof(T)] = message = new MessageConsumeContext<T>(this, messageToken as T);
                    return true;
                }

                var typeUrn = MessageUrn.ForTypeString<T>();

                if (_supportedTypes.Any(x => typeUrn.Equals(x, StringComparison.OrdinalIgnoreCase)))
                {
                    var deserializeType = typeof(T);
                    if (deserializeType.GetTypeInfo().IsInterface && TypeMetadataCache<T>.IsValidMessageType)
                        deserializeType = TypeMetadataCache<T>.ImplementationType;

                    var obj = messageToken.ToObject<T>(SystemTextJsonMessageSerializer.Options);
                    _messageTypes[typeof(T)] = message = new MessageConsumeContext<T>(this, obj);

                    return true;
                }

                _messageTypes[typeof(T)] = message = null;
                return false;
            }
        }

        static JsonElement GetMessageToken(object message)
        {
            return message is JsonElement element
                ? element.ValueKind == JsonValueKind.Null
                    ? new JsonElement()
                    : element
                : new JsonElement();
        }

        static Guid? ConvertIdToGuid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return default;

            if (Guid.TryParse(id, out var messageId))
                return messageId;

            throw new FormatException("The Id was not a Guid: " + id);
        }

        static Uri ConvertToUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
                return null;

            return new Uri(uri);
        }
    }
}
