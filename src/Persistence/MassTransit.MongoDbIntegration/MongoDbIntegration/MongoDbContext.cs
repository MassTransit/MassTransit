#nullable enable
namespace MassTransit.MongoDbIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MongoDB.Driver;


    public interface MongoDbContext :
        IDisposable
    {
        /// <summary>
        /// The currently active session, if so started
        /// </summary>
        IClientSessionHandle? Session { get; }

        Task<IClientSessionHandle> StartSession(CancellationToken cancellationToken);

        Task BeginTransaction(CancellationToken cancellationToken);
        Task CommitTransaction(CancellationToken cancellationToken);
        Task AbortTransaction(CancellationToken cancellationToken);

        MongoDbCollectionContext<T> GetCollection<T>();
    }
}
