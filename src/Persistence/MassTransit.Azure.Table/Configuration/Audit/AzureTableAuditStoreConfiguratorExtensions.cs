namespace MassTransit
{
    using System;
    using Azure.Table;
    using Microsoft.Azure.Cosmos.Table;


    public static class AzureTableAuditStoreConfiguratorExtensions
    {
        /// <summary>
        /// Supply your storage account and table name for audit logs. Default Partition Key Strategy and no filters will be applied.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="storageAccount">Your cloud storage account.</param>
        /// <param name="auditTableName">The name of the table for which the Audit Logs will be persisted.</param>
        public static void UseAzureTableAuditStore(this IBusFactoryConfigurator configurator, CloudStorageAccount storageAccount, string auditTableName)
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(auditTableName);
            table.CreateIfNotExists();
            ConfigureAuditStore(configurator, table, null, DefaultPartitionKeyStrategy);
        }

        /// <summary>
        /// Supply your storage account, table name and filter to be used. Default Partition Key Strategy will be applied.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="storageAccount">Your cloud storage account.</param>
        /// <param name="auditTableName">The name of the table for which the Audit Logs will be persisted.</param>
        /// <param name="configureFilter">Message Filter to exclude or include messages from audit based on requirements</param>
        public static void UseAzureTableAuditStore(this IBusFactoryConfigurator configurator, CloudStorageAccount storageAccount, string auditTableName,
            Action<IMessageFilterConfigurator> configureFilter)
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(auditTableName);
            table.CreateIfNotExists();
            ConfigureAuditStore(configurator, table, configureFilter, DefaultPartitionKeyStrategy);
        }

        /// <summary>
        ///  Supply your storage account, table name and partition key strategy based on the message type and audit information. No Filters will be applied.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="storageAccount">Your cloud storage account.</param>
        /// <param name="auditTableName">The name of the table for which the Audit Logs will be persisted.</param>
        /// <param name="partitionKeyStrategy">Using the message type and audit information or otherwise, specify the partition key strategy</param>
        public static void UseAzureTableAuditStore(this IBusFactoryConfigurator configurator, CloudStorageAccount storageAccount, string auditTableName,
            Func<string, AuditRecord, string> partitionKeyStrategy)
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(auditTableName);
            table.CreateIfNotExists();
            ConfigureAuditStore(configurator, table, null, partitionKeyStrategy);
        }

        /// <summary>
        ///  Supply your storage account, table name, partition key strategy and message filter to be applied.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="storageAccount">Your cloud storage account.</param>
        /// <param name="auditTableName">The name of the table for which the Audit Logs will be persisted.</param>
        /// <param name="partitionKeyStrategy">Using the message type and audit information or otherwise, specify the partition key strategy</param>
        /// <param name="configureFilter">Message Filter to exclude or include messages from audit based on requirements</param>
        public static void UseAzureTableAuditStore(this IBusFactoryConfigurator configurator, CloudStorageAccount storageAccount, string auditTableName,
            Func<string, AuditRecord, string> partitionKeyStrategy, Action<IMessageFilterConfigurator> configureFilter)
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(auditTableName);
            table.CreateIfNotExists();
            Func<string, AuditRecord, string> pkStrategy = partitionKeyStrategy ?? DefaultPartitionKeyStrategy;
            ConfigureAuditStore(configurator, table, configureFilter, pkStrategy);
        }

        /// <summary>
        ///  Supply your table, partition key strategy and message filter to be applied.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="table">Your Azure Cloud Table</param>
        /// <param name="partitionKeyStrategy">Using the message type and audit information or otherwise, specify the partition key strategy</param>
        /// <param name="configureFilter">Message Filter to exclude or include messages from audit based on requirements</param>
        public static void UseAzureTableAuditStore(this IBusFactoryConfigurator configurator, CloudTable table,
            Func<string, AuditRecord, string> partitionKeyStrategy, Action<IMessageFilterConfigurator> configureFilter)
        {
            Func<string, AuditRecord, string> pkStrategy = partitionKeyStrategy ?? DefaultPartitionKeyStrategy;
            ConfigureAuditStore(configurator, table, configureFilter, pkStrategy);
        }

        /// <summary>
        ///  Supply your table and message filter to be applied. Default Partition Key Strategy will be used.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="table">Your Azure Cloud Table</param>
        /// <param name="configureFilter">Message Filter to exclude or include messages from audit based on requirements</param>
        public static void UseAzureTableAuditStore(this IBusFactoryConfigurator configurator, CloudTable table,
            Action<IMessageFilterConfigurator> configureFilter)
        {
            ConfigureAuditStore(configurator, table, configureFilter, DefaultPartitionKeyStrategy);
        }

        /// <summary>
        ///  Supply your table and partition key strategy. No message filter will be applied
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="table">Your Azure Cloud Table</param>
        /// <param name="partitionKeyStrategy">Using the message type and audit information or otherwise, specify the partition key strategy</param>
        public static void UseAzureTableAuditStore(this IBusFactoryConfigurator configurator, CloudTable table,
            Func<string, AuditRecord, string> partitionKeyStrategy)
        {
            ConfigureAuditStore(configurator, table, null, partitionKeyStrategy);
        }

        public static void UseAzureTableAuditStore(this IBusFactoryConfigurator configurator, CloudTable table)
        {
            ConfigureAuditStore(configurator, table, null, DefaultPartitionKeyStrategy);
        }

        static void ConfigureAuditStore(IBusFactoryConfigurator configurator, CloudTable table, Action<IMessageFilterConfigurator> configureFilter,
            Func<string, AuditRecord, string> partitionKeyStrategy)
        {
            configurator.ConnectBusObserver(new AzureTableAuditBusObserver(table, configureFilter, partitionKeyStrategy));
        }

        static string DefaultPartitionKeyStrategy(string messageType, AuditRecord record)
        {
            return record.ContextType;
        }
    }
}
