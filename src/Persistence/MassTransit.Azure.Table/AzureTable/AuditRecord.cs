namespace MassTransit.AzureTable
{
    using System;
    using System.Text;
    using System.Text.Json;
    using Audit;
    using Microsoft.Azure.Cosmos.Table;
    using Serialization;


    public class AuditRecord :
        TableEntity
    {
        static readonly char[] _disallowedCharacters;

        static AuditRecord()
        {
            _disallowedCharacters = new[] { '/', '\\', '#', '?' };
        }

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
        public string Custom { get; set; }
        public string Headers { get; set; }
        public string Message { get; set; }

        internal static AuditRecord Create<T>(T message, MessageAuditMetadata metadata, IPartitionKeyFormatter partitionKeyFormatter)
            where T : class
        {
            var record = new AuditRecord
            {
                RowKey = $"{DateTime.MaxValue.Subtract(metadata.SentTime ?? DateTime.UtcNow).TotalMilliseconds}",
                ContextType = metadata.ContextType,
                MessageId = metadata.MessageId,
                ConversationId = metadata.ConversationId,
                CorrelationId = metadata.CorrelationId,
                InitiatorId = metadata.InitiatorId,
                RequestId = metadata.RequestId,
                SentTime = metadata.SentTime,
                SourceAddress = metadata.SourceAddress,
                InputAddress = metadata.InputAddress,
                DestinationAddress = metadata.DestinationAddress,
                ResponseAddress = metadata.ResponseAddress,
                FaultAddress = metadata.FaultAddress,
                Headers = JsonSerializer.Serialize(metadata.Headers, SystemTextJsonMessageSerializer.Options),
                Custom = JsonSerializer.Serialize(metadata.Custom, SystemTextJsonMessageSerializer.Options),
                Message = JsonSerializer.Serialize(message, SystemTextJsonMessageSerializer.Options),
                MessageType = TypeCache<T>.ShortName
            };

            record.PartitionKey = SanitizePartitionKey(partitionKeyFormatter.Format<T>(record));

            return record;
        }

        static string SanitizePartitionKey(string partitionKey)
        {
            if (partitionKey.IndexOfAny(_disallowedCharacters) < 0)
                return partitionKey;

            var key = new StringBuilder(partitionKey.Length);

            foreach (var c in partitionKey)
            {
                switch (c)
                {
                    case '/':
                    case '\\':
                    case '#':
                    case '?':
                        continue;

                    default:
                        key.Append(c);
                        break;
                }
            }

            return key.ToString();
        }
    }
}
