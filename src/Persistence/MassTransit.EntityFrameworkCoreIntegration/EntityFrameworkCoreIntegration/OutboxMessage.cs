#nullable enable
namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Collections.Generic;
    using Metadata;
    using Middleware;
    using Serialization;


    public class OutboxMessage :
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

        /// <summary>
        /// Used for inbox + outbox only messages, which are by consumer
        /// </summary>
        public Guid? InboxMessageId { get; set; }

        public Guid? InboxConsumerId { get; set; }

        /// <summary>
        /// Used for outbox (on-ramp) only messages, which are on a separate index
        /// </summary>
        public Guid? OutboxId { get; set; }

        public long SequenceNumber { get; set; }

        public Guid MessageId { get; set; }

        public string ContentType { get; set; } = null!;
        public string MessageType { get; set; } = null!;

        public string Body { get; set; } = null!;

        public Guid? ConversationId { get; set; }
        public Guid? CorrelationId { get; set; }
        public Guid? InitiatorId { get; set; }

        public Guid? RequestId { get; set; }

        public Uri? SourceAddress { get; set; }
        public Uri? DestinationAddress { get; set; }
        public Uri? ResponseAddress { get; set; }
        public Uri? FaultAddress { get; set; }

        /// <summary>
        /// If the message is not delivered to the transport within the expiration time, consider moving to a dead-letter queue instead
        /// </summary>
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
            Dictionary<string, object?>? headers = deserializer.DeserializeDictionary<object?>(Headers);
            if (headers != null)
                return new DictionarySendHeaders(headers);

            return EmptyHeaders.Instance;
        }

        IReadOnlyDictionary<string, object> DeserializerProperties(IObjectDeserializer deserializer)
        {
            Dictionary<string, object>? properties = deserializer.DeserializeDictionary<object>(Properties);

            return properties ?? OutboxMessageStaticData.Empty;
        }
    }


    static class OutboxMessageStaticData
    {
        public static IReadOnlyDictionary<string, object> Empty { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    }
}
