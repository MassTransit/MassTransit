namespace MassTransit.AzureTable
{
    using System.Threading.Tasks;
    using Audit;
    using Microsoft.Azure.Cosmos.Table;


    public class AzureTableAuditStore :
        IMessageAuditStore
    {
        readonly IPartitionKeyFormatter _partitionKeyFormatter;
        readonly CloudTable _table;

        public AzureTableAuditStore(CloudTable table, IPartitionKeyFormatter partitionKeyFormatter)
        {
            _table = table;
            _partitionKeyFormatter = partitionKeyFormatter;
        }

        Task IMessageAuditStore.StoreMessage<T>(T message, MessageAuditMetadata metadata)
        {
            var auditRecord = AuditRecord.Create(message, metadata, _partitionKeyFormatter);
            var insertOrMergeOperation = TableOperation.InsertOrMerge(auditRecord);
            return _table.ExecuteAsync(insertOrMergeOperation);
        }
    }
}
