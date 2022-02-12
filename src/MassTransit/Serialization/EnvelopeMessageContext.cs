#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using Metadata;


    public class EnvelopeMessageContext :
        MessageContext
    {
        readonly MessageEnvelope _envelope;
        readonly IObjectDeserializer _objectDeserializer;
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

        public EnvelopeMessageContext(MessageEnvelope envelope, IObjectDeserializer objectDeserializer)
        {
            _envelope = envelope;
            _objectDeserializer = objectDeserializer;
        }

        public Guid? MessageId => _messageId ??= ConvertIdToGuid(_envelope.MessageId);
        public Guid? RequestId => _requestId ??= ConvertIdToGuid(_envelope.RequestId);
        public Guid? CorrelationId => _correlationId ??= ConvertIdToGuid(_envelope.CorrelationId);
        public Guid? ConversationId => _conversationId ??= ConvertIdToGuid(_envelope.ConversationId);
        public Guid? InitiatorId => _initiatorId ??= ConvertIdToGuid(_envelope.InitiatorId);
        public DateTime? ExpirationTime => _envelope.ExpirationTime;
        public Uri? SourceAddress => _sourceAddress ??= ConvertToUri(_envelope.SourceAddress);
        public Uri? DestinationAddress => _destinationAddress ??= ConvertToUri(_envelope.DestinationAddress);
        public Uri? ResponseAddress => _responseAddress ??= ConvertToUri(_envelope.ResponseAddress);
        public Uri? FaultAddress => _faultAddress ??= ConvertToUri(_envelope.FaultAddress);
        public DateTime? SentTime => _envelope.SentTime;
        public Headers Headers => _headers ??= GetHeaders();
        public HostInfo Host => _envelope.Host ?? HostMetadataCache.Empty;

        Headers GetHeaders()
        {
            return _envelope.Headers != null
                ? (Headers)new ReadOnlyDictionaryHeaders(_objectDeserializer, _envelope.Headers)
                : EmptyHeaders.Instance;
        }

        static Guid? ConvertIdToGuid(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return default;

            if (Guid.TryParse(id, out var messageId))
                return messageId;

            throw new FormatException("The Id was not a Guid: " + id);
        }

        static Uri? ConvertToUri(string? uri)
        {
            return string.IsNullOrWhiteSpace(uri) ? null : new Uri(uri);
        }
    }
}
