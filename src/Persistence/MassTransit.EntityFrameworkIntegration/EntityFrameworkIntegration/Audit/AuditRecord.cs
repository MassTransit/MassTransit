namespace MassTransit.EntityFrameworkIntegration.Audit
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json;
    using MassTransit.Audit;
    using Serialization;


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

        internal string _custom { get; set; }

        [NotMapped]
        public Dictionary<string, string> Custom
        {
            get =>
                string.IsNullOrEmpty(_custom)
                    ? new Dictionary<string, string>()
                    : JsonSerializer.Deserialize<Dictionary<string, string>>(_custom, SystemTextJsonMessageSerializer.Options);
            set => _custom = JsonSerializer.Serialize(value, SystemTextJsonMessageSerializer.Options);
        }

        internal string _headers { get; set; }

        [NotMapped]
        public Dictionary<string, string> Headers
        {
            get =>
                string.IsNullOrEmpty(_headers)
                    ? new Dictionary<string, string>()
                    : JsonSerializer.Deserialize<Dictionary<string, string>>(_headers, SystemTextJsonMessageSerializer.Options);
            set => _headers = JsonSerializer.Serialize(value, SystemTextJsonMessageSerializer.Options);
        }

        internal string _message { get; set; }

        [NotMapped]
        public object Message
        {
            get =>
                string.IsNullOrEmpty(_message)
                    ? null
                    : JsonSerializer.Deserialize<object>(_message, SystemTextJsonMessageSerializer.Options);
            set => _message = JsonSerializer.Serialize(value, SystemTextJsonMessageSerializer.Options);
        }

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
                FaultAddress = metadata.FaultAddress,
                Headers = metadata.Headers,
                Custom = metadata.Custom,
                Message = message,
                MessageType = messageType
            };
        }
    }
}
