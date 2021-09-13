using MassTransit.Metadata;
using System;
using System.Collections.Generic;

namespace MassTransit.Serialization
{
    public class XmlMessageEnvelope
      : MessageEnvelope
    {
        protected XmlMessageEnvelope()
        {

        }

        public XmlMessageEnvelope(SendContext context, object message, string[] messageTypeNames)
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

        public string MessageId { get; private set; }
        public string RequestId { get; private set; }
        public string CorrelationId { get; private set; }
        public string ConversationId { get; private set; }
        public string InitiatorId { get; private set; }
        public string SourceAddress { get; private set; }
        public string DestinationAddress { get; private set; }
        public string ResponseAddress { get; private set; }
        public string FaultAddress { get; private set; }
        public string[] MessageType { get; private set; }
        public object Message { get; private set; }
        public DateTime? ExpirationTime { get; private set; }
        public DateTime? SentTime { get; private set; }
        public IDictionary<string, object> Headers { get; private set; }
        public HostInfo Host { get; private set; }


    }
}
