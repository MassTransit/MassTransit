#nullable enable
namespace MassTransit.MongoDbIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MongoDB.Driver;


    public class TransactionMongoDbCollectionContext<T> :
        MongoDbCollectionContext<T>
    {
        static readonly InsertOneOptions _insertOneOptions = new InsertOneOptions();
        readonly IMongoCollection<T> _collection;
        readonly MongoDbContext _context;

        public TransactionMongoDbCollectionContext(MongoDbContext context, IMongoCollection<T> collection)
        {
            _context = context;
            _collection = collection;
        }

        public Task InsertOne(T instance, CancellationToken cancellationToken)
        {
            return _context.Session != null
                ? _collection.InsertOneAsync(_context.Session, instance, _insertOneOptions, cancellationToken)
                : _collection.InsertOneAsync(instance, _insertOneOptions, cancellationToken);
        }

        public Task<T> FindOneAndReplace(FilterDefinition<T> filter, T instance, CancellationToken cancellationToken)
        {
            return _context.Session != null
                ? _collection.FindOneAndReplaceAsync(_context.Session, filter, instance, null, cancellationToken)
                : _collection.FindOneAndReplaceAsync(filter, instance, null, cancellationToken);
        }

        public Task<DeleteResult> DeleteOne(FilterDefinition<T> filter, CancellationToken cancellationToken)
        {
            return _context.Session != null
                ? _collection.DeleteOneAsync(_context.Session, filter, null, cancellationToken)
                : _collection.DeleteOneAsync(filter, null, cancellationToken);
        }

        public Task<DeleteResult> DeleteMany(FilterDefinition<T> filter, CancellationToken cancellationToken)
        {
            return _context.Session != null
                ? _collection.DeleteManyAsync(_context.Session, filter, null, cancellationToken)
                : _collection.DeleteManyAsync(filter, null, cancellationToken);
        }

        public async Task<T> Lock(FilterDefinition<T> filter, UpdateDefinition<T> update, CancellationToken cancellationToken)
        {
            var session = _context.Session;
            if (session == null)
                throw new InvalidOperationException("Lock requires an active session");

            var options = new FindOneAndUpdateOptions<T> { ReturnDocument = ReturnDocument.After };

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    return await _collection.FindOneAndUpdateAsync(session, filter, update, options, cancellationToken).ConfigureAwait(false);
                }
                catch (MongoCommandException e) when (e.CodeName == "WriteConflict" && e.HasErrorLabel("TransientTransactionError"))
                {
                    if (session.IsInTransaction)
                        await session.AbortTransactionAsync(cancellationToken).ConfigureAwait(false);

                    await Task.Delay(TimeSpan.FromMilliseconds(10), cancellationToken).ConfigureAwait(false);

                    session.StartTransaction();
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            throw new OperationCanceledException("The document could not be locked");
        }

        public IFindFluent<T, T> Find(FilterDefinition<T> filter)
        {
            return _context.Session != null
                ? _collection.Find(_context.Session, filter)
                : _collection.Find(filter);
        }
    }
}
