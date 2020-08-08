namespace MassTransit
{
    using System;
    using EntityFrameworkCoreIntegration.Audit;
    using Microsoft.EntityFrameworkCore;


    public static class EntityFrameworkAuditStoreConfiguratorExtensions
    {
        /// <summary>
        /// Configure Audit Store against Entity Framework Core
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="dbContextOptions">The DB Context Options configuration builder</param>
        /// <param name="auditTableName">Name of the table to store audit records.</param>
        /// <param name="configureFilter">Configure messages to exclude or include from auditing.</param>
        public static void UseEntityFrameworkCoreAuditStore(this IBusFactoryConfigurator configurator, DbContextOptionsBuilder dbContextOptions,
            string auditTableName,
            Action<IMessageFilterConfigurator> configureFilter)
        {
            ConfigureAuditStore(configurator, dbContextOptions, auditTableName, configureFilter);
        }

        /// <summary>
        /// Configure Audit Store against Entity Framework Core
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="dbContextOptions">The DB Context Options configuration builder</param>
        /// <param name="auditTableName">Name of the table to store audit records.</param>
        public static void UseEntityFrameworkCoreAuditStore(this IBusFactoryConfigurator configurator, DbContextOptionsBuilder dbContextOptions,
            string auditTableName)
        {
            ConfigureAuditStore(configurator, dbContextOptions, auditTableName);
        }

        static void ConfigureAuditStore(IBusFactoryConfigurator configurator, DbContextOptionsBuilder dbContextOptions, string auditTableName,
            Action<IMessageFilterConfigurator> configureFilter = default)
        {
            var auditStore = new EntityFrameworkAuditStore(dbContextOptions.Options, auditTableName);

            configurator.ConnectSendAuditObservers(auditStore, configureFilter);
            configurator.ConnectConsumeAuditObserver(auditStore, configureFilter);
        }
    }
}
