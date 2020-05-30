namespace MassTransit.Azure.Cosmos.Table
{
    using System;
    using Audit;


    public static class AzureCosmosTableAuditStoreConfiguratorExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString"></param>
        /// <param name="auditTableName"></param>
        /// <param name="configureFilter"></param>
        public static void UseAzureCosmosTableAuditStore(this IBusFactoryConfigurator configurator, string connectionString, string auditTableName, Action<IMessageFilterConfigurator> configureFilter = null)
        {
            configurator.ConnectBusObserver(new AzureCosmosTableAuditBusObserver(connectionString, auditTableName, configureFilter, DefaultPartitionKeyStrategy));
        }

        /// <summary>
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString"></param>
        /// <param name="auditTableName"></param>
        /// <param name="configureFilter"></param>
        /// <param name="partitionKeyStrategy"></param>
        public static void UseAzureCosmosTableAuditStore(this IBusFactoryConfigurator configurator, string connectionString, string auditTableName, Action<IMessageFilterConfigurator> configureFilter = null,
                                                         Func<string, AuditRecord, string> partitionKeyStrategy)
        {
            configurator.ConnectBusObserver(new AzureCosmosTableAuditBusObserver(connectionString, auditTableName, configureFilter, partitionKeyStrategy));
        }

        static string DefaultPartitionKeyStrategy(string messageType, AuditRecord record)
        {
            return record.ContextType;
        }
    }
}
