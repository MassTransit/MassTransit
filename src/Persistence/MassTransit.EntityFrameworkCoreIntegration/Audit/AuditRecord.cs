namespace MassTransit.EntityFrameworkCoreIntegration.Audit
{
    using System;
    using System.Collections.Generic;
    using MassTransit.Audit;


    public class AuditRecord
    {
        public int AuditRecordId { get; set; }
        public Guid? MessageId { get; set; }
        public Guid? ConversationId { get; set; }
        public Guid? CorrelationId { get; set; }
        public Guid? InitiatorId { get; set; }
        public Guid? RequestId { get; set; }
        public DateTime? SentTime { get; set; }
        public string SourceAddress { get; set; }
        public string DestinationAddress { get; set; }
        public string ResponseAddress { get; set; }
        public string FaultAddress { get; set; }
        public string InputAddress { get; set; }
        public string ContextType { get; set; }
        public string MessageType { get; set; }
        public Dictionary<string, string> Custom { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public object Message { get; set; }

        internal static AuditRecord Create<T>(T message, string messageType, MessageAuditMetadata metadata)
            where T : class
        {
            return new AuditRecord
            {
                ContextType = metadata.ContextType,
                MessageId = metadata.MessageId,
                ConversationId = metadata.ConversationId,
                CorrelationId = metadata.CorrelationId,
                InitiatorId = metadata.InitiatorId,
                RequestId = metadata.RequestId,
                SentTime = metadata.SentTime,
                SourceAddress = metadata.SourceAddress,
                DestinationAddress = metadata.DestinationAddress,
                ResponseAddress = metadata.ResponseAddress,
                InputAddress = metadata.InputAddress,
                FaultAddress = metadata.FaultAddress,
                Headers = metadata.Headers,
                Custom = metadata.Custom,
                Message = message,
                MessageType = messageType
            };
        }
    }
}
