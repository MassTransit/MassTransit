namespace MassTransit
{
    using System;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using MongoDbIntegration;
    using MongoDbIntegration.Saga;


    public interface IMongoDbSagaRepositoryConfigurator
    {
        /// <summary>
        /// Sets the database factory using connection string <see cref="MongoUrl" />
        /// </summary>
        string Connection { set; }

        /// <summary>
        /// Sets the database name
        /// </summary>
        string DatabaseName { set; }

        /// <summary>
        /// Sets the collection name using <see cref="DefaultCollectionNameFormatter" />
        /// </summary>
        string CollectionName { set; }

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
    }


    public interface IMongoDbSagaRepositoryConfigurator<TSaga> :
        IMongoDbSagaRepositoryConfigurator
        where TSaga : class, ISagaVersion
    {
        /// <summary>
        /// Configure class map using <see cref="BsonClassMap{TClass}" />
        /// </summary>
        /// <param name="classMapFactory"></param>
        void ClassMap(Func<IServiceProvider, BsonClassMap<TSaga>> classMapFactory);
    }
}
