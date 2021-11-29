namespace MassTransit
{
    using System;
    using MongoDB.Driver;
    using MongoDbIntegration.Audit;


    public static class MongoDbAuditStoreConfiguratorExtensions
    {
        /// <summary>
        /// Supply your database connection and table name
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString">MongoDB server connection string</param>
        /// <param name="databaseName">The database to use for auditing</param>
        /// <param name="collectionName">The name of the collection to store audit logs</param>
        /// <param name="configureFilter">Message Filter to exclude or include messages from audit based on requirements</param>
        public static void UseMongoDbAuditStore(this IBusFactoryConfigurator configurator, string connectionString, string databaseName, string collectionName,
            Action<IMessageFilterConfigurator> configureFilter = default)
        {
            var auditStore = new MongoDbAuditStore(connectionString, databaseName, collectionName);
            ConfigureAuditStore(configurator, auditStore, configureFilter);
        }

        /// <summary>
        /// Supply your database connection and table name
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="mongoUrl">MongoDB URL</param>
        /// <param name="collectionName">The name of the collection to store audit logs</param>
        /// <param name="configureFilter">Message Filter to exclude or include messages from audit based on requirements</param>
        public static void UseMongoDbAuditStore(this IBusFactoryConfigurator configurator, MongoUrl mongoUrl, string collectionName,
            Action<IMessageFilterConfigurator> configureFilter = default)
        {
            var auditStore = new MongoDbAuditStore(mongoUrl, collectionName);
            ConfigureAuditStore(configurator, auditStore, configureFilter);
        }

        /// <summary>
        /// Supply your database connection and table name
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="mongoDatabase">MongoDB connection</param>
        /// <param name="collectionName">The name of the collection to store audit logs</param>
        /// <param name="configureFilter">Message Filter to exclude or include messages from audit based on requirements</param>
        public static void UseMongoDbAuditStore(this IBusFactoryConfigurator configurator, IMongoDatabase mongoDatabase, string collectionName,
            Action<IMessageFilterConfigurator> configureFilter = default)
        {
            var auditStore = new MongoDbAuditStore(mongoDatabase, collectionName);
            ConfigureAuditStore(configurator, auditStore, configureFilter);
        }

        static void ConfigureAuditStore(IBusFactoryConfigurator configurator, MongoDbAuditStore auditStore, Action<IMessageFilterConfigurator> configureFilter = default)
        {
            configurator.ConnectSendAuditObservers(auditStore, configureFilter);
            configurator.ConnectConsumeAuditObserver(auditStore, configureFilter);
        }
    }
}
