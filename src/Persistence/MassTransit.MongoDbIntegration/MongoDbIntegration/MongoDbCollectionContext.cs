#nullable enable
namespace MassTransit.MongoDbIntegration
{
    using System.Threading;
    using System.Threading.Tasks;
    using MongoDB.Driver;


    /// <summary>
    /// Encapsulates <see cref="IMongoCollection{T}" /> so that it can be including an existing session/transaction
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface MongoDbCollectionContext<T>
    {
        Task InsertOne(T instance, CancellationToken cancellationToken);
        Task<ReplaceOneResult> ReplaceOne(FilterDefinition<T> filter, T instance, CancellationToken cancellationToken);
        Task<T> FindOneAndReplace(FilterDefinition<T> filter, T instance, CancellationToken cancellationToken);
        Task<DeleteResult> DeleteOne(FilterDefinition<T> filter, CancellationToken cancellationToken);
        Task<DeleteResult> DeleteMany(FilterDefinition<T> filter, CancellationToken cancellationToken);

        Task<T> Lock(FilterDefinition<T> filter, UpdateDefinition<T> update, CancellationToken cancellationToken);

        IFindFluent<T, T> Find(FilterDefinition<T> filter);
    }
}
