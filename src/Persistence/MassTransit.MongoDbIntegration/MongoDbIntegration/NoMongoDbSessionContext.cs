#nullable enable
namespace MassTransit.MongoDbIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using MongoDB.Driver;


    /// <summary>
    /// No transaction support, since no client is available
    /// </summary>
    public class NoMongoDbSessionContext :
        MongoDbContext
    {
        readonly IServiceProvider _provider;

        public NoMongoDbSessionContext(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IClientSessionHandle? Session => null;
        public Guid? TransactionId => null;

        public Task<IClientSessionHandle> StartSession(CancellationToken cancellationToken)
        {
            throw new NotSupportedException("To use sessions, configure the ClientFactory in the MongoDb repository/outbox configuration");
        }

        public Task BeginTransaction(CancellationToken cancellationToken)
        {
            throw new NotSupportedException("To use transactions, configure the ClientFactory in the MongoDb repository/outbox configuration");
        }

        public Task CommitTransaction(CancellationToken cancellationToken)
        {
            throw new NotSupportedException("To use transactions, configure the ClientFactory in the MongoDb repository/outbox configuration");
        }

        public Task AbortTransaction(CancellationToken cancellationToken)
        {
            throw new NotSupportedException("To use transactions, configure the ClientFactory in the MongoDb repository/outbox configuration");
        }

        public MongoDbCollectionContext<T> GetCollection<T>()
        {
            return new NoSessionMongoDbCollectionContext<T>(_provider.GetRequiredService<IMongoCollection<T>>());
        }

        public void Dispose()
        {
        }
    }
}
