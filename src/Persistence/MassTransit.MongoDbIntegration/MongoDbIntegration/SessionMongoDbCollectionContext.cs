#nullable enable
namespace MassTransit.MongoDbIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MongoDB.Driver;


    public class SessionMongoDbCollectionContext<T> :
        MongoDbCollectionContext<T>
    {
        static readonly InsertOneOptions _insertOneOptions = new InsertOneOptions();
        readonly IMongoCollection<T> _collection;
        readonly MongoDbSessionOptions _options;
        readonly IClientSessionHandle _session;

        public SessionMongoDbCollectionContext(IClientSessionHandle session, IMongoCollection<T> collection, MongoDbSessionOptions options)
        {
            _session = session;
            _options = options;
            _collection = collection;
        }

        public Task InsertOne(T instance, CancellationToken cancellationToken)
        {
            return _collection.InsertOneAsync(_session, instance, _insertOneOptions, cancellationToken);
        }

        public Task<T> FindOneAndReplace(FilterDefinition<T> filter, T instance, CancellationToken cancellationToken)
        {
            return _collection.FindOneAndReplaceAsync(_session, filter, instance, null, cancellationToken);
        }

        public Task<DeleteResult> DeleteOne(FilterDefinition<T> filter, CancellationToken cancellationToken)
        {
            return _collection.DeleteOneAsync(_session, filter, null, cancellationToken);
        }

        public Task<DeleteResult> DeleteMany(FilterDefinition<T> filter, CancellationToken cancellationToken)
        {
            return _collection.DeleteManyAsync(_session, filter, null, cancellationToken);
        }

        public IFindFluent<T, T> Find(FilterDefinition<T> filter)
        {
            return _collection.Find(_session, filter);
        }

        public async Task<T> Lock(FilterDefinition<T> filter, UpdateDefinition<T> update, CancellationToken cancellationToken)
        {
            var options = new FindOneAndUpdateOptions<T> { ReturnDocument = ReturnDocument.After };

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    return await _collection.FindOneAndUpdateAsync(_session, filter, update, options, cancellationToken).ConfigureAwait(false);
                }
                catch (MongoCommandException e) when (e.CodeName == "WriteConflict" && e.HasErrorLabel("TransientTransactionError"))
                {
                    await _session.AbortTransactionAsync(cancellationToken).ConfigureAwait(false);

                    await Task.Delay(_options.LockRetryDelay, cancellationToken).ConfigureAwait(false);

                    _session.StartTransaction();
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            throw new OperationCanceledException("The document could not be locked");
        }
    }
}
