#nullable enable
namespace MassTransit.Middleware.Outbox
{
    using System;
    using System.Collections.Generic;
    using Metadata;
    using Serialization;


    public class InMemoryOutboxMessage :
        OutboxMessageContext
    {
        Headers? _headers;
        IReadOnlyDictionary<string, object>? _properties;

        /// <summary>
        /// When the message should be visible / ready to be delivered
        /// </summary>
        public DateTime? EnqueueTime { get; set; }

        public DateTime SentTime { get; set; }

        public string? Headers { get; set; }

        /// <summary>
        /// Transport-specific message properties (routing key, partition key, sessionId, etc.)
        /// </summary>
        public string? Properties { get; set; }

        public long SequenceNumber { get; set; }

        public Guid MessageId { get; set; }

        public string ContentType { get; set; } = null!;
        public string Body { get; set; } = null!;

        public Guid? ConversationId { get; set; }
        public Guid? CorrelationId { get; set; }
        public Guid? InitiatorId { get; set; }
        public Guid? RequestId { get; set; }

        public Uri? SourceAddress { get; set; }
        public Uri? DestinationAddress { get; set; }
        public Uri? ResponseAddress { get; set; }
        public Uri? FaultAddress { get; set; }

        public DateTime? ExpirationTime { get; set; }

        Guid? MessageContext.MessageId => MessageId;
        DateTime? MessageContext.SentTime => SentTime;
        Headers MessageContext.Headers => _headers ?? EmptyHeaders.Instance;
        HostInfo MessageContext.Host => HostMetadataCache.Host;

        IReadOnlyDictionary<string, object> OutboxMessageContext.Properties => _properties!;

        public void Deserialize(IObjectDeserializer deserializer)
        {
            _headers = DeserializerHeaders(deserializer);
            _properties = DeserializerProperties(deserializer);
        }

        Headers DeserializerHeaders(IObjectDeserializer deserializer)
        {
            if (string.IsNullOrWhiteSpace(Properties))
                return EmptyHeaders.Instance;

            var dictionary = deserializer.DeserializeObject<Dictionary<string, object?>>(Properties);
            if (dictionary != null && dictionary.Count > 0)
                return new DictionarySendHeaders(dictionary);

            return EmptyHeaders.Instance;
        }

        IReadOnlyDictionary<string, object> DeserializerProperties(IObjectDeserializer deserializer)
        {
            if (string.IsNullOrWhiteSpace(Properties))
                return OutboxMessageStaticData.Empty;

            return deserializer.DeserializeObject<Dictionary<string, object>>(Properties) ?? OutboxMessageStaticData.Empty;
        }
    }


    static class OutboxMessageStaticData
    {
        public static IReadOnlyDictionary<string, object> Empty { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    }
}
