#nullable enable
namespace MassTransit.MongoDbIntegration
{
    using System.Threading;
    using System.Threading.Tasks;
    using MongoDB.Driver;


    public class NoSessionMongoDbCollectionContext<T> :
        MongoDbCollectionContext<T>
    {
        static readonly InsertOneOptions _options = new InsertOneOptions();
        readonly IMongoCollection<T> _collection;

        public NoSessionMongoDbCollectionContext(IMongoCollection<T> collection)
        {
            _collection = collection;
        }

        public IMongoCollection<T> Collection => _collection;

        public Task InsertOne(T instance, CancellationToken cancellationToken)
        {
            return _collection.InsertOneAsync(instance, _options, cancellationToken);
        }

        public Task<T> FindOneAndReplace(FilterDefinition<T> filter, T instance, CancellationToken cancellationToken)
        {
            return _collection.FindOneAndReplaceAsync(filter, instance, null, cancellationToken);
        }

        public Task<DeleteResult> DeleteOne(FilterDefinition<T> filter, CancellationToken cancellationToken)
        {
            return _collection.DeleteOneAsync(filter, null, cancellationToken);
        }

        public Task<DeleteResult> DeleteMany(FilterDefinition<T> filter, CancellationToken cancellationToken)
        {
            return _collection.DeleteManyAsync(filter, null, cancellationToken);
        }

        public IFindFluent<T, T> Find(FilterDefinition<T> filter)
        {
            return _collection.Find(filter);
        }

        public Task<T> Lock(FilterDefinition<T> filter, UpdateDefinition<T> update, CancellationToken cancellationToken)
        {
            return _collection.FindOneAndUpdateAsync(filter, update, null, cancellationToken);
        }
    }
}
