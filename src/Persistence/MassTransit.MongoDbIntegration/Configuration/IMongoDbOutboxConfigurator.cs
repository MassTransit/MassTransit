#nullable enable
namespace MassTransit
{
    using System;
    using MongoDB.Driver;
    using MongoDbIntegration;


    public interface IMongoDbOutboxConfigurator :
        ITransactionalOutboxConfigurator
    {
        /// <summary>
        /// The amount of time a message remains in the inbox for duplicate detection (based on MessageId)
        /// </summary>
        public TimeSpan DuplicateDetectionWindow { set; }

        /// <summary>
        /// The delay between queries once messages are no longer available. When a query returns messages, subsequent queries
        /// are performed until no messages are returned after which the QueryDelay is used.
        /// </summary>
        public TimeSpan QueryDelay { set; }

        /// <summary>
        /// The maximum number of messages to query from the database at a time
        /// </summary>
        public int QueryMessageLimit { set; }

        /// <summary>
        /// Database query timeout
        /// </summary>
        public TimeSpan QueryTimeout { set; }

        /// <summary>
        /// Sets the database factory using connection string <see cref="MongoUrl" />
        /// </summary>
        string Connection { set; }

        /// <summary>
        /// Sets the database name
        /// </summary>
        string DatabaseName { set; }

        /// <summary>
        /// Use the configuration service provider to resolve the collection name formatter <see cref="ICollectionNameFormatter" />
        /// </summary>
        /// <param name="collectionNameFormatterFactory"></param>
        void CollectionNameFormatter(Func<IServiceProvider, ICollectionNameFormatter> collectionNameFormatterFactory);

        /// <summary>
        /// Use the configuration service provider to resolve the database <see cref="IMongoDatabase" />
        /// </summary>
        /// <param name="databaseFactory"></param>
        void DatabaseFactory(Func<IServiceProvider, IMongoDatabase> databaseFactory);

        /// <summary>
        /// Use the configuration service provider to resolve the MongoDB client <see cref="IMongoClient" />
        /// </summary>
        /// <param name="clientFactory"></param>
        void ClientFactory(Func<IServiceProvider, IMongoClient> clientFactory);

        /// <summary>
        /// Disable the inbox cleanup service, removing the hosted service from the service collection
        /// </summary>
        void DisableInboxCleanupService();

        /// <summary>
        /// The Bus Outbox intercepts the <see cref="ISendEndpointProvider" /> and <see cref="IPublishEndpoint" /> interfaces
        /// that are used when not consuming messages. Messages sent or published via those interfaces are written to the outbox
        /// instead of being delivered directly to the message broker.
        /// </summary>
        void UseBusOutbox(Action<IMongoDbBusOutboxConfigurator>? configure = null);
    }
}
