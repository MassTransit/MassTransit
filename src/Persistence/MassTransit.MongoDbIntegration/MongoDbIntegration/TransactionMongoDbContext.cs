#nullable enable
namespace MassTransit.MongoDbIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using MongoDB.Driver;


    /// <summary>
    /// Allows the creation of a session with a transaction
    /// </summary>
    public class TransactionMongoDbContext :
        MongoDbContext
    {
        static readonly ClientSessionOptions _sessionOptions = new ClientSessionOptions
        {
            DefaultTransactionOptions = new TransactionOptions(ReadConcern.Majority, ReadPreference.Primary, WriteConcern.WMajority)
        };

        readonly IMongoClient _client;
        readonly MongoDbSessionOptions _options;
        readonly IServiceProvider _provider;

        public TransactionMongoDbContext(IMongoClient client, IServiceProvider provider, IOptions<MongoDbSessionOptions> options)
        {
            _client = client;
            _provider = provider;

            _options = options.Value;
        }

        public IClientSessionHandle? Session { get; private set; }

        public void Dispose()
        {
            Session?.Dispose();
        }

        public async Task<IClientSessionHandle> StartSession(CancellationToken cancellationToken)
        {
            return Session ??= await _client.StartSessionAsync(_sessionOptions, cancellationToken).ConfigureAwait(false);
        }

        public async Task BeginTransaction(CancellationToken cancellationToken)
        {
            Session ??= await _client.StartSessionAsync(_sessionOptions, cancellationToken).ConfigureAwait(false);

            Session.StartTransaction();
        }

        public Task CommitTransaction(CancellationToken cancellationToken)
        {
            if (Session == null)
                throw new InvalidOperationException("No session has been created");

            if (Session.IsInTransaction == false)
                throw new InvalidOperationException("The session is not in an active transaction");

            return Session.CommitTransactionAsync(cancellationToken);
        }

        public Task AbortTransaction(CancellationToken cancellationToken)
        {
            if (Session == null)
                throw new InvalidOperationException("No session has been created");

            if (Session.IsInTransaction == false)
                throw new InvalidOperationException("The session is not in an active transaction");

            return Session.AbortTransactionAsync(cancellationToken);
        }

        public MongoDbCollectionContext<T> GetCollection<T>()
        {
            var collection = _provider.GetRequiredService<IMongoCollection<T>>();

            return Session != null
                ? (MongoDbCollectionContext<T>)new SessionMongoDbCollectionContext<T>(Session, collection, _options)
                : new NoSessionMongoDbCollectionContext<T>(collection);
        }
    }
}
