namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;

    public class SystemTextJsonMessageEnvelope : MessageEnvelope
    {
        public string MessageId { get; set; }
        public string RequestId { get; set; }
        public string CorrelationId { get; set; }
        public string ConversationId { get; set; }
        public string InitiatorId { get; set; }
        public string SourceAddress { get; set; }
        public string DestinationAddress { get; set; }
        public string ResponseAddress { get; set; }
        public string FaultAddress { get; set; }
        public string[] MessageType { get; set; }
        public object Message { get; set; }
        public DateTime? ExpirationTime { get; set; }
        public DateTime? SentTime { get; set; }
        public IDictionary<string, object> Headers { get; set; }
        public HostInfo Host { get; set; }
    }
}
