namespace MassTransit.MongoDbIntegration
{
    using MongoDB.Driver;


    public interface ICollectionNameFormatter
    {
        string Saga<TSaga>()
            where TSaga : ISaga;
    }


    public static class CollectionNameFormatterExtensions
    {
        public static IMongoCollection<TSaga> GetCollection<TSaga>(this IMongoDatabase database, ICollectionNameFormatter collectionNameFormatter)
            where TSaga : class, ISaga
        {
            return database.GetCollection<TSaga>(collectionNameFormatter.Saga<TSaga>());
        }
    }
}
