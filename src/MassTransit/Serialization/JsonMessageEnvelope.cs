#nullable enable
namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using Metadata;


    [Serializable]
    public class JsonMessageEnvelope :
        MessageEnvelope
    {
        Dictionary<string, object?>? _headers;

        public JsonMessageEnvelope()
        {
        }

        public JsonMessageEnvelope(SendContext context, object message, string[] messageTypeNames)
        {
            if (context.MessageId.HasValue)
                MessageId = context.MessageId.Value.ToString();

            if (context.RequestId.HasValue)
                RequestId = context.RequestId.Value.ToString();

            if (context.CorrelationId.HasValue)
                CorrelationId = context.CorrelationId.Value.ToString();

            if (context.ConversationId.HasValue)
                ConversationId = context.ConversationId.Value.ToString();

            if (context.InitiatorId.HasValue)
                InitiatorId = context.InitiatorId.Value.ToString();

            if (context.SourceAddress != null)
                SourceAddress = context.SourceAddress.ToString();

            if (context.DestinationAddress != null)
                DestinationAddress = context.DestinationAddress.ToString();

            if (context.ResponseAddress != null)
                ResponseAddress = context.ResponseAddress.ToString();

            if (context.FaultAddress != null)
                FaultAddress = context.FaultAddress.ToString();

            MessageType = messageTypeNames;

            Message = message;

            if (context.TimeToLive.HasValue)
                ExpirationTime = DateTime.UtcNow + context.TimeToLive;

            SentTime = context.SentTime ?? DateTime.UtcNow;

            Headers = new Dictionary<string, object?>();

            foreach (KeyValuePair<string, object> header in context.Headers.GetAll())
                Headers[header.Key] = header.Value;

            Host = HostMetadataCache.Host;
        }

        public JsonMessageEnvelope(MessageContext context, object message, string[] messageTypeNames)
        {
            if (context.MessageId.HasValue)
                MessageId = context.MessageId.Value.ToString();

            if (context.RequestId.HasValue)
                RequestId = context.RequestId.Value.ToString();

            if (context.CorrelationId.HasValue)
                CorrelationId = context.CorrelationId.Value.ToString();

            if (context.ConversationId.HasValue)
                ConversationId = context.ConversationId.Value.ToString();

            if (context.InitiatorId.HasValue)
                InitiatorId = context.InitiatorId.Value.ToString();

            if (context.SourceAddress != null)
                SourceAddress = context.SourceAddress.ToString();

            if (context.DestinationAddress != null)
                DestinationAddress = context.DestinationAddress.ToString();

            if (context.ResponseAddress != null)
                ResponseAddress = context.ResponseAddress.ToString();

            if (context.FaultAddress != null)
                FaultAddress = context.FaultAddress.ToString();

            MessageType = messageTypeNames;

            Message = message;

            if (context.ExpirationTime.HasValue)
                ExpirationTime = context.ExpirationTime;

            SentTime = context.SentTime ?? DateTime.UtcNow;

            Headers = new Dictionary<string, object?>();

            foreach (KeyValuePair<string, object> header in context.Headers.GetAll())
                Headers[header.Key] = header.Value;

            Host = HostMetadataCache.Host;
        }

        public JsonMessageEnvelope(MessageEnvelope envelope)
        {
            MessageId = envelope.MessageId;
            RequestId = envelope.RequestId;
            CorrelationId = envelope.CorrelationId;
            ConversationId = envelope.ConversationId;
            InitiatorId = envelope.InitiatorId;
            SourceAddress = envelope.SourceAddress;
            DestinationAddress = envelope.DestinationAddress;
            ResponseAddress = envelope.ResponseAddress;
            FaultAddress = envelope.FaultAddress;

            MessageType = envelope.MessageType;
            Message = envelope.Message;

            ExpirationTime = envelope.ExpirationTime;

            SentTime = envelope.SentTime ?? DateTime.UtcNow;

            Headers = envelope.Headers != null
                ? new Dictionary<string, object?>(envelope.Headers, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

            Host = envelope.Host ?? HostMetadataCache.Host;
        }

        public string? MessageId { get; set; }
        public string? RequestId { get; set; }
        public string? CorrelationId { get; set; }
        public string? ConversationId { get; set; }
        public string? InitiatorId { get; set; }
        public string? SourceAddress { get; set; }
        public string? DestinationAddress { get; set; }
        public string? ResponseAddress { get; set; }
        public string? FaultAddress { get; set; }
        public string[]? MessageType { get; set; }
        public object? Message { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public DateTime? SentTime { get; set; }

        public Dictionary<string, object?> Headers
        {
            get => _headers ??= new Dictionary<string, object?>();
            set => _headers = value;
        }

        public HostInfo? Host { get; set; }

        public void Update(SendContext context)
        {
            DestinationAddress = context.DestinationAddress?.ToString();

            if (context.SourceAddress != null)
                SourceAddress = context.SourceAddress.ToString();

            if (context.ResponseAddress != null)
                ResponseAddress = context.ResponseAddress.ToString();

            if (context.FaultAddress != null)
                FaultAddress = context.FaultAddress.ToString();

            if (context.MessageId.HasValue)
                MessageId = context.MessageId.ToString();

            if (context.RequestId.HasValue)
                RequestId = context.RequestId.ToString();

            if (context.ConversationId.HasValue)
                ConversationId = context.ConversationId.ToString();

            if (context.CorrelationId.HasValue)
                CorrelationId = context.CorrelationId.ToString();

            if (context.InitiatorId.HasValue)
                InitiatorId = context.InitiatorId.ToString();

            if (context.TimeToLive.HasValue)
                ExpirationTime = DateTime.UtcNow + (context.TimeToLive > TimeSpan.Zero ? context.TimeToLive : TimeSpan.FromSeconds(1));

            foreach (KeyValuePair<string, object> header in context.Headers.GetAll())
                Headers[header.Key] = header.Value;
        }
    }
}
