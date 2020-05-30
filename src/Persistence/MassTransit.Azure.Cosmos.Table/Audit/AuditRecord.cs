namespace MassTransit.Azure.Cosmos.Table
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Audit;
    using Microsoft.Azure.Cosmos.Table;
    using Newtonsoft.Json;


    public class AuditRecord : TableEntity
    {
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

        internal static AuditRecord Create<T>(T message, string messageType, MessageAuditMetadata metadata,
                                              Func<string, AuditRecord, string> partitionKeyStrategy)
            where T : class
        {
            var record = new AuditRecord
                         {
                             RowKey =
                                 $"{DateTime.MaxValue.Subtract(metadata.SentTime ?? DateTime.UtcNow).TotalMilliseconds}",
                             ContextType        = metadata.ContextType,
                             MessageId          = metadata.MessageId,
                             ConversationId     = metadata.ConversationId,
                             CorrelationId      = metadata.CorrelationId,
                             InitiatorId        = metadata.InitiatorId,
                             RequestId          = metadata.RequestId,
                             SentTime           = metadata.SentTime,
                             SourceAddress      = metadata.SourceAddress,
                             InputAddress       = metadata.InputAddress,
                             DestinationAddress = metadata.DestinationAddress,
                             ResponseAddress    = metadata.ResponseAddress,
                             FaultAddress       = metadata.FaultAddress,
                             Headers            = JsonConvert.SerializeObject(metadata.Headers),
                             Custom             = JsonConvert.SerializeObject(metadata.Custom),
                             Message            = JsonConvert.SerializeObject(message),
                             MessageType        = messageType
                         };
            record.PartitionKey = CleanDisallowedPartitionKeyCharacters(partitionKeyStrategy.Invoke(messageType, record));
            return record;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="candidatePartitionKey"></param>
        /// <returns></returns>
        static string CleanDisallowedPartitionKeyCharacters(string candidatePartitionKey)
        {
            var disallowedCharacters = new HashSet<char>(){'/', '\\', '#', '?'};
            var key = new StringBuilder();
            foreach (var character in candidatePartitionKey)
            {
                if (!disallowedCharacters.Contains(character))
                    key.Append(character);
            }

            return key.ToString();
        }
    }
}
