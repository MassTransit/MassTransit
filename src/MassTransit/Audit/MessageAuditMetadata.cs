namespace MassTransit.Audit
{
    using System;
    using System.Collections.Generic;


    public class MessageAuditMetadata
    {
        public Guid? MessageId { get; set; }
        public Guid? ConversationId { get; set; }
        public Guid? CorrelationId { get; set; }
        public Guid? InitiatorId { get; set; }
        public Guid? RequestId { get; set; }
        public DateTime? SentTime { get; set; }
        public string SourceAddress { get; set; }
        public string DestinationAddress { get; set; }
        public string InputAddress { get; set; }
        public string ResponseAddress { get; set; }
        public string FaultAddress { get; set; }
        public string ContextType { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, string> Custom { get; set; }
    }
}
