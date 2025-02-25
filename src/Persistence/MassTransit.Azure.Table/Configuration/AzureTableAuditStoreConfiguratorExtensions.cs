namespace MassTransit
{
    using System;
    using Azure.Data.Tables;
    using AzureTable;


    public static class AzureTableAuditStoreConfiguratorExtensions
    {
        /// <summary>
        /// Supply your storage account and table name for audit logs. Default Partition Key Strategy and no filters will be applied.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="tableServiceClient">Service client used to perform all service level operations.</param>
        /// <param name="auditTableName">The name of the table for which the Audit Logs will be persisted.</param>
        public static void UseAzureTableAuditStore(this IBusFactoryConfigurator configurator, TableServiceClient tableServiceClient, string auditTableName)
        {
            var tableClient = tableServiceClient.GetTableClient(auditTableName);
            tableClient.CreateIfNotExists();

            ConfigureAuditStore(configurator, tableClient);
        }

        /// <summary>
        /// Supply your storage account, table name and filter to be used. Default Partition Key Strategy will be applied.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="tableServiceClient">Service client used to perform all service level operations.</param>
        /// <param name="auditTableName">The name of the table for which the Audit Logs will be persisted.</param>
        /// <param name="configureFilter">Message Filter to exclude or include messages from audit based on requirements</param>
        public static void UseAzureTableAuditStore(this IBusFactoryConfigurator configurator, TableServiceClient tableServiceClient, string auditTableName,
            Action<IMessageFilterConfigurator> configureFilter)
        {
            var tableClient = tableServiceClient.GetTableClient(auditTableName);
            tableClient.CreateIfNotExists();

            ConfigureAuditStore(configurator, tableClient, configureFilter);
        }

        /// <summary>
        /// Supply your storage account, table name and partition key strategy based on the message type and audit information. No Filters will be applied.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="tableServiceClient">Service client used to perform all service level operations.</param>
        /// <param name="auditTableName">The name of the table for which the Audit Logs will be persisted.</param>
        /// <param name="partitionKeyFormatter">
        /// Using the message type and audit information or otherwise, specify the partition key strategy
        /// </param>
        public static void UseAzureTableAuditStore(this IBusFactoryConfigurator configurator, TableServiceClient tableServiceClient, string auditTableName,
            IPartitionKeyFormatter partitionKeyFormatter)
        {
            var tableClient = tableServiceClient.GetTableClient(auditTableName);
            tableClient.CreateIfNotExists();

            ConfigureAuditStore(configurator, tableClient, null, partitionKeyFormatter);
        }

        /// <summary>
        /// Supply your storage account, table name, partition key strategy and message filter to be applied.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="tableServiceClient">Service client used to perform all service level operations.</param>
        /// <param name="auditTableName">The name of the table for which the Audit Logs will be persisted.</param>
        /// <param name="partitionKeyFormatter">
        /// Using the message type and audit information or otherwise, specify the partition key strategy
        /// </param>
        /// <param name="configureFilter">Message Filter to exclude or include messages from audit based on requirements</param>
        public static void UseAzureTableAuditStore(this IBusFactoryConfigurator configurator, TableServiceClient tableServiceClient, string auditTableName,
            IPartitionKeyFormatter partitionKeyFormatter, Action<IMessageFilterConfigurator> configureFilter)
        {
            var tableClient = tableServiceClient.GetTableClient(auditTableName);
            tableClient.CreateIfNotExists();

            ConfigureAuditStore(configurator, tableClient, configureFilter, partitionKeyFormatter);
        }

        /// <summary>
        /// Supply your table, partition key strategy and message filter to be applied.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="table">Your Azure Cloud Table</param>
        /// <param name="partitionKeyFormatter">
        /// Using the message type and audit information or otherwise, specify the partition key strategy
        /// </param>
        /// <param name="configureFilter">Message Filter to exclude or include messages from audit based on requirements</param>
        public static void UseAzureTableAuditStore(this IBusFactoryConfigurator configurator, TableClient table,
            IPartitionKeyFormatter partitionKeyFormatter, Action<IMessageFilterConfigurator> configureFilter)
        {
            ConfigureAuditStore(configurator, table, configureFilter, partitionKeyFormatter);
        }

        /// <summary>
        /// Supply your table and message filter to be applied. Default Partition Key Strategy will be used.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="table">Your Azure Cloud Table</param>
        /// <param name="configureFilter">Message Filter to exclude or include messages from audit based on requirements</param>
        public static void UseAzureTableAuditStore(this IBusFactoryConfigurator configurator, TableClient table,
            Action<IMessageFilterConfigurator> configureFilter)
        {
            ConfigureAuditStore(configurator, table, configureFilter);
        }

        /// <summary>
        /// Supply your table and partition key strategy. No message filter will be applied
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="table">Your Azure Cloud Table</param>
        /// <param name="partitionKeyFormatter">
        /// Using the message type and audit information or otherwise, specify the partition key strategy
        /// </param>
        public static void UseAzureTableAuditStore(this IBusFactoryConfigurator configurator, TableClient table, IPartitionKeyFormatter partitionKeyFormatter)
        {
            ConfigureAuditStore(configurator, table, null, partitionKeyFormatter);
        }

        public static void UseAzureTableAuditStore(this IBusFactoryConfigurator configurator, TableClient table)
        {
            ConfigureAuditStore(configurator, table);
        }

        static void ConfigureAuditStore(IBusFactoryConfigurator configurator, TableClient table, Action<IMessageFilterConfigurator> configureFilter = default,
            IPartitionKeyFormatter formatter = default)
        {
            var auditStore = new AzureTableAuditStore(table, formatter ?? new DefaultPartitionKeyFormatter());

            configurator.ConnectSendAuditObservers(auditStore, configureFilter);
            configurator.ConnectConsumeAuditObserver(auditStore, configureFilter);
        }
    }
}
