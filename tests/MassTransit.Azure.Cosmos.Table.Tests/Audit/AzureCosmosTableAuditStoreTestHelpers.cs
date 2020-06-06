namespace MassTransit.Azure.Cosmos.Table.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Table;


    class AzureCosmosTableAuditStoreTestHelpers
    {
        readonly CloudTable _table;

        public AzureCosmosTableAuditStoreTestHelpers(string connectionString, string auditTableName)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            _table = tableClient.GetTableReference(auditTableName);
            _table.CreateIfNotExists();
        }

        public IEnumerable<AuditRecord> GetAuditRecords()
        {
            var query = new TableQuery<AuditRecord>();
            return _table.ExecuteQuery(query);
        }

        public Task<bool> RemoveAuditTable()
        {
            return _table.DeleteIfExistsAsync();
        }
    }
}
