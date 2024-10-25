namespace MassTransit.AzureTable
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Audit;
    using Azure.Data.Tables;


    public class AzureTableAuditStore :
        IMessageAuditStore
    {
        readonly IPartitionKeyFormatter _partitionKeyFormatter;
        readonly TableClient _table;

        public AzureTableAuditStore(TableClient table, IPartitionKeyFormatter partitionKeyFormatter)
        {
            _table = table;
            _partitionKeyFormatter = partitionKeyFormatter;
        }

        Task IMessageAuditStore.StoreMessage<T>(T message, MessageAuditMetadata metadata)
        {
            var auditRecord = AuditRecord.Create(message, metadata, _partitionKeyFormatter);
            var p = new Dictionary<string, object>();
            p.Add(auditRecord.PartitionKey, auditRecord);
            TableEntity entity = new TableEntity(p);
            return _table.UpsertEntityAsync(entity);
        }
    }
}
