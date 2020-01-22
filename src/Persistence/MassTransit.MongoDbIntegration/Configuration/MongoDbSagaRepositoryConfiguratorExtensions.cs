namespace MassTransit.MongoDbIntegration
{
    using MongoDB.Driver;
    using Saga.CollectionNameFormatters;


    public static class MongoDbSagaRepositoryConfiguratorExtensions
    {
        /// <summary>
        /// Sets the collection name formatter instance <see cref="ICollectionNameFormatter"/>
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="collectionNameFormatter"></param>
        public static void CollectionNameFormatter(this IMongoDbSagaRepositoryConfigurator configurator, ICollectionNameFormatter collectionNameFormatter)
        {
            configurator.CollectionNameFormatter(_ => collectionNameFormatter);
        }

        /// <summary>
        /// Sets the database factory using the database instance <see cref="IMongoDatabase"/>
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="database"></param>
        public static void Database(this IMongoDbSagaRepositoryConfigurator configurator, IMongoDatabase database)
        {
            configurator.DatabaseFactory(_ => database);
        }
    }
}
