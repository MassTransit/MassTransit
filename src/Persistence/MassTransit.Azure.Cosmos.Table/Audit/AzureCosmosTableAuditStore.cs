namespace MassTransit.Azure.Cosmos.Table
{
    using System;
    using System.Threading.Tasks;
    using Audit;
    using Metadata;
    using Microsoft.Azure.Cosmos.Table;


    public class AzureCosmosTableAuditStore : IMessageAuditStore
    {
        readonly Func<string, AuditRecord, string> _partitionKeyStrategy;
        readonly CloudTable _table;

        public AzureCosmosTableAuditStore(CloudTable table, Func<string, AuditRecord, string> partitionKeyStrategy)
        {
            _table                = table;
            _partitionKeyStrategy = partitionKeyStrategy;
        }

        async Task IMessageAuditStore.StoreMessage<T>(T message, MessageAuditMetadata metadata)
        {
            var auditRecord            = AuditRecord.Create(message, TypeMetadataCache<T>.ShortName, metadata, _partitionKeyStrategy);
            var insertOrMergeOperation = TableOperation.InsertOrMerge(auditRecord);
            await _table.ExecuteAsync(insertOrMergeOperation).ConfigureAwait(false);
        }
    }
}
