namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using Metadata;


    public class JsonMessageEnvelope :
        MessageEnvelope
    {
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

            Headers = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> header in context.Headers.GetAll())
                Headers[header.Key] = header.Value;

            Host = HostMetadataCache.Host;
        }

        public string MessageId { get; }
        public string RequestId { get; }
        public string CorrelationId { get; }
        public string ConversationId { get; }
        public string InitiatorId { get; }
        public string SourceAddress { get; }
        public string DestinationAddress { get; }
        public string ResponseAddress { get; }
        public string FaultAddress { get; }
        public string[] MessageType { get; }
        public object Message { get; }
        public DateTime? ExpirationTime { get; }
        public DateTime? SentTime { get; }
        public IDictionary<string, object> Headers { get; }
        public HostInfo Host { get; }
    }
}
