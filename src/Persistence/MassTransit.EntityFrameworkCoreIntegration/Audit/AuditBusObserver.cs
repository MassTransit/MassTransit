namespace MassTransit.EntityFrameworkCoreIntegration.Audit
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes.Util;
    using Microsoft.EntityFrameworkCore;


    public class AuditBusObserver :
        IBusObserver
    {
        readonly string _auditTableName;
        readonly DbContextOptionsBuilder _dbContextOptionsBuilder;
        readonly Action<IMessageFilterConfigurator> _filter;

        public AuditBusObserver(DbContextOptionsBuilder dbContextOptions, string auditTableName, Action<IMessageFilterConfigurator> filter)
        {
            _filter = filter;
            _dbContextOptionsBuilder = dbContextOptions;
            _auditTableName = auditTableName;
        }

        public async Task PostCreate(IBus bus)
        {
            LogContext.Debug?.Log($"Connecting Entity Framework Core Audit Store {_auditTableName}");

            var auditStore = new EntityFrameworkAuditStore(_dbContextOptionsBuilder.Options, _auditTableName);

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
