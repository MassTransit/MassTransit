#nullable enable
namespace MassTransit.MongoDbIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
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
        readonly object _lock = new object();
        readonly IServiceProvider _provider;

        Task<IClientSessionHandle>? _session;

        public TransactionMongoDbContext(IMongoClient client, IServiceProvider provider)
        {
            _client = client;
            _provider = provider;
        }

        public IClientSessionHandle? Session { get; private set; }

        public Guid? TransactionId { get; private set; }

        public void Dispose()
        {
            Session?.Dispose();
        }

        public Task<IClientSessionHandle> StartSession(CancellationToken cancellationToken)
        {
            async Task<IClientSessionHandle> Start()
            {
                var handle = await _client.StartSessionAsync(_sessionOptions, cancellationToken).ConfigureAwait(false);

                Session = handle;

                return handle;
            }

            lock (_lock)
            {
                if (_session != null)
                    return _session;

                _session = Start();

                return _session;
            }
        }

        public async Task BeginTransaction(CancellationToken cancellationToken)
        {
            var session = await StartSession(cancellationToken).ConfigureAwait(false);

            if (!session.IsInTransaction)
            {
                session.StartTransaction();
                TransactionId = NewId.NextGuid();
            }
        }

        public async Task CommitTransaction(CancellationToken cancellationToken)
        {
            if (Session == null)
                throw new InvalidOperationException("No session has been created");

            if (Session.IsInTransaction == false)
                throw new InvalidOperationException("The session is not in an active transaction");

            await Session.CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
            TransactionId = null;
        }

        public async Task AbortTransaction(CancellationToken cancellationToken)
        {
            if (Session == null)
                throw new InvalidOperationException("No session has been created");

            if (Session.IsInTransaction == false)
                throw new InvalidOperationException("The session is not in an active transaction");

            await Session.AbortTransactionAsync(cancellationToken).ConfigureAwait(false);
            TransactionId = null;
        }

        public MongoDbCollectionContext<T> GetCollection<T>()
        {
            var collection = _provider.GetRequiredService<IMongoCollection<T>>();

            return new TransactionMongoDbCollectionContext<T>(this, collection);
        }
    }
}
