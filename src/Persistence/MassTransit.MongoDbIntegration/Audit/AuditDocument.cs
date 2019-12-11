namespace MassTransit.MongoDbIntegration.Audit
{
    using System;
    using MassTransit.Audit;
    using Newtonsoft.Json;


    public class AuditDocument
    {
        public string MessageId { get; set; }
        public string ConversationId { get; set; }
        public string CorrelationId { get; set; }
        public string InitiatorId { get; set; }
        public string RequestId { get; set; }
        public DateTime? SentTime { get; set; }
        public string SourceAddress { get; set; }
        public string DestinationAddress { get; set; }
        public string ResponseAddress { get; set; }
        public string FaultAddress { get; set; }
        public string InputAddress { get; set; }
        public string ContextType { get; set; }
        public string MessageType { get; set; }
        public AuditHeaders Headers { get; set; }
        public AuditHeaders Custom { get; set; }
        public string Message { get; set; }

        internal static AuditDocument Create<T>(T message, string messageType, MessageAuditMetadata metadata)
            where T : class
        {
            return new AuditDocument
            {
                ContextType = metadata.ContextType,
                MessageId = metadata.MessageId.ToString(),
                ConversationId = metadata.ConversationId.ToString(),
                CorrelationId = metadata.CorrelationId.ToString(),
                InitiatorId = metadata.InitiatorId.ToString(),
                RequestId = metadata.RequestId.ToString(),
                SentTime = metadata.SentTime,
                SourceAddress = metadata.SourceAddress,
                DestinationAddress = metadata.DestinationAddress,
                ResponseAddress = metadata.ResponseAddress,
                InputAddress = metadata.InputAddress,
                FaultAddress = metadata.FaultAddress,
                Message = JsonConvert.SerializeObject(message),
                MessageType = messageType,
                Headers = AuditHeaders.FromDictionary(metadata.Headers),
                Custom = AuditHeaders.FromDictionary(metadata.Custom)
            };
        }
    }
}
