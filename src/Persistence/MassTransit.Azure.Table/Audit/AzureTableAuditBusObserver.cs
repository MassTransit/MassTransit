namespace MassTransit.Azure.Table.Audit
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes.Util;
    using Microsoft.Azure.Cosmos.Table;


    public class AzureTableAuditBusObserver :
        IBusObserver
    {
        readonly Action<IMessageFilterConfigurator> _filter;
        readonly IPartitionKeyFormatter _partitionKeyFormatter;
        readonly CloudTable _table;

        public AzureTableAuditBusObserver(CloudTable table, Action<IMessageFilterConfigurator> filter, IPartitionKeyFormatter partitionKeyFormatter)
        {
            _table = table;
            _filter = filter;
            _partitionKeyFormatter = partitionKeyFormatter;
        }

        public async Task PostCreate(IBus bus)
        {
            LogContext.Debug?.Log($"Connecting Azure Table Audit Store: {_table.Name}");

            var auditStore = new AzureTableAuditStore(_table, _partitionKeyFormatter);

            bus.ConnectSendAuditObservers(auditStore, _filter);
            bus.ConnectConsumeAuditObserver(auditStore, _filter);
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
