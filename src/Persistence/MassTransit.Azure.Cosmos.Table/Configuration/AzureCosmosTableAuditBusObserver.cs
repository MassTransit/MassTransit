namespace MassTransit.Azure.Cosmos.Table
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes.Util;
    using Microsoft.Azure.Cosmos.Table;


    public class AzureCosmosTableAuditBusObserver :
        IBusObserver
    {
        readonly string _auditTableName;
        readonly string _connectionString;
        readonly Func<string, AuditRecord, string> _partitionKeyStrategy;
        readonly Action<IMessageFilterConfigurator> _filter;

        public AzureCosmosTableAuditBusObserver(string connectionString, string auditTableName, Action<IMessageFilterConfigurator> filter, Func<string, AuditRecord, string> partitionKeyStrategy)
        {
            _connectionString = connectionString;
            _auditTableName   = auditTableName;
            _partitionKeyStrategy = partitionKeyStrategy;
            _filter = filter;
        }

        public async Task PostCreate(IBus bus)
        {
            LogContext.Debug?.Log($"Connecting Azure Cosmos Table Audit Store against table {_auditTableName}");
            var table = await GetAuditCloudTable(_auditTableName);
            var auditStore = new AzureCosmosTableAuditStore(table, _partitionKeyStrategy);
            bus.ConnectSendAuditObservers(auditStore, _filter);
            bus.ConnectConsumeAuditObserver(auditStore, _filter);
            LogContext.Debug?.Log($"Azure Cosmos Table Audit store connected. {_auditTableName}");
        }

        async Task<CloudTable> GetAuditCloudTable(string auditTableName)
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var tableClient    = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            var table          = tableClient.GetTableReference(auditTableName);
            await table.CreateIfNotExistsAsync().ConfigureAwait(false);
            return table;
        }

        public Task CreateFaulted(Exception exception)
        {
            return TaskUtil.Completed;
        }

        public Task PreStart(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            return TaskUtil.Completed;
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }

        public Task PreStop(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public Task PostStop(IBus bus)
        {
            return TaskUtil.Completed;
        }

        public Task StopFaulted(IBus bus, Exception exception)
        {
            return TaskUtil.Completed;
        }
    }
}
