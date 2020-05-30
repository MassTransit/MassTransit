namespace MassTransit.Azure.Cosmos.Table
{
    using System;


    public static class AzureCosmosTableAuditStoreConfiguratorExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString"></param>
        /// <param name="auditTableName"></param>
        /// <param name="configureFilter"></param>
        public static void UseAzureCosmosTableAuditStore(this IBusFactoryConfigurator configurator, string connectionString, string auditTableName,
                                                         Action<IMessageFilterConfigurator> configureFilter = null)
        {
            configurator.ConnectBusObserver(new AzureCosmosTableAuditBusObserver(connectionString,auditTableName,
                                                                                 configureFilter,
                                                                                 DefaultPartitionKeyStrategy));
        }

        /// <summary>
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString">Connection string to storage instance</param>
        /// <param name="auditTableName">Table name within Cosmos to store audit records</param>
        /// <param name="partitionKeyStrategy">Construct Partition key from message type & audit record information</param>
        /// <param name="configureFilter">Message Filter</param>
        public static void UseAzureCosmosTableAuditStore(this IBusFactoryConfigurator configurator, string connectionString, string auditTableName,
                                                         Func<string, AuditRecord, string> partitionKeyStrategy,
                                                         Action<IMessageFilterConfigurator> configureFilter = null)
        {
            configurator.ConnectBusObserver(new AzureCosmosTableAuditBusObserver(connectionString, auditTableName, configureFilter, partitionKeyStrategy));
        }

        static string DefaultPartitionKeyStrategy(string messageType, AuditRecord record)
        {
            return record.ContextType;
        }
    }
}
