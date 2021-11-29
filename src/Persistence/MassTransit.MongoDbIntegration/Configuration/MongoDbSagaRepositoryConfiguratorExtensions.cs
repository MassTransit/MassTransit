namespace MassTransit
{
    using System;
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using MongoDbIntegration;
    using MongoDbIntegration.Saga;


    public static class MongoDbSagaRepositoryConfiguratorExtensions
    {
        /// <summary>
        /// Sets the collection name formatter instance <see cref="ICollectionNameFormatter" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="collectionNameFormatter"></param>
        public static void CollectionNameFormatter(this IMongoDbSagaRepositoryConfigurator configurator, ICollectionNameFormatter collectionNameFormatter)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (collectionNameFormatter == null)
                throw new ArgumentNullException(nameof(collectionNameFormatter));

            configurator.CollectionNameFormatter(_ => collectionNameFormatter);
        }

        /// <summary>
        /// Sets the database factory using the database instance <see cref="IMongoDatabase" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="database"></param>
        public static void Database(this IMongoDbSagaRepositoryConfigurator configurator, IMongoDatabase database)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (database == null)
                throw new ArgumentNullException(nameof(database));

            configurator.DatabaseFactory(_ => database);
        }

        /// <summary>
        /// Configure saga class mapping using <see cref="BsonClassMap{TClass}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="classMapConfigurator"></param>
        /// <typeparam name="TSaga"></typeparam>
        public static void ClassMap<TSaga>(this IMongoDbSagaRepositoryConfigurator<TSaga> configurator,
            Action<BsonClassMap<TSaga>> classMapConfigurator)
            where TSaga : class, ISagaVersion
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (classMapConfigurator == null)
                throw new ArgumentNullException(nameof(classMapConfigurator));

            configurator.ClassMap(_ => classMapConfigurator);
        }

        /// <summary>
        /// Configure saga class mapping using <see cref="BsonClassMap{TClass}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="classMapConfigurator"></param>
        /// <typeparam name="TSaga"></typeparam>
        public static void ClassMap<TSaga>(this IMongoDbSagaRepositoryConfigurator<TSaga> configurator,
            Func<IServiceProvider, Action<BsonClassMap<TSaga>>> classMapConfigurator)
            where TSaga : class, ISagaVersion
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (classMapConfigurator == null)
                throw new ArgumentNullException(nameof(classMapConfigurator));

            configurator.ClassMap(provider => new BsonClassMap<TSaga>(classMapConfigurator(provider)));
        }

        /// <summary>
        /// Configure saga class mapping using <see cref="BsonClassMap{TClass}" />
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="classMap"></param>
        /// <typeparam name="TSaga"></typeparam>
        public static void ClassMap<TSaga>(this IMongoDbSagaRepositoryConfigurator<TSaga> configurator, BsonClassMap<TSaga> classMap)
            where TSaga : class, ISagaVersion
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            if (classMap == null)
                throw new ArgumentNullException(nameof(classMap));

            configurator.ClassMap(_ => classMap);
        }
    }
}
