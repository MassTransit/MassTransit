#nullable enable
namespace MassTransit.MongoDbIntegration
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Clients;
    using DependencyInjection;
    using Middleware;
    using Middleware.Outbox;
    using Outbox;
    using Serialization;
    using Transports;


    public class MongoDbScopedBusContext<TBus> :
        ScopedBusContext,
        OutboxSendContext,
        IDisposable
        where TBus : class, IBus
    {
        readonly TBus _bus;
        readonly IClientFactory _clientFactory;
        readonly MongoDbContext _dbContext;
        readonly SemaphoreSlim _lock;
        readonly IBusOutboxNotification _notification;
        readonly MongoDbCollectionContext<OutboxMessage> _outboxMessages;
        readonly MongoDbCollectionContext<OutboxState> _outboxStates;
        readonly IServiceProvider _provider;

        IPublishEndpoint? _publishEndpoint;
        IScopedClientFactory? _scopedClientFactory;
        ISendEndpointProvider? _sendEndpointProvider;
        Guid? _transactionId;

        public MongoDbScopedBusContext(TBus bus, MongoDbContext dbContext, IBusOutboxNotification notification, IClientFactory clientFactory,
            IServiceProvider provider)
        {
            _bus = bus;
            _dbContext = dbContext;
            _notification = notification;
            _clientFactory = clientFactory;
            _provider = provider;

            _outboxMessages = _dbContext.GetCollection<OutboxMessage>();
            _outboxStates = _dbContext.GetCollection<OutboxState>();

            _lock = new SemaphoreSlim(1);
        }

        public void Dispose()
        {
            _lock.Wait();
            if (WasCommitted())
                _notification.Delivered();

            _transactionId = null;
            _lock.Dispose();
        }

        public async Task AddSend<T>(SendContext<T> context)
            where T : class
        {
            if (_transactionId == null || WasCommitted())
            {
                await _lock.WaitAsync(context.CancellationToken).ConfigureAwait(false);
                try
                {
                    if (WasCommitted())
                    {
                        _notification.Delivered();
                        _transactionId = null;
                    }

                    _transactionId ??= await StartOutboxTransaction(context.CancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    _lock.Release();
                }
            }

            await _outboxMessages.AddSend(context, SystemTextJsonMessageSerializer.Instance, outboxId: _transactionId).ConfigureAwait(false);
        }

        public object? GetService(Type serviceType)
        {
            return _provider.GetService(serviceType);
        }

        public ISendEndpointProvider SendEndpointProvider => _sendEndpointProvider ??= new OutboxSendEndpointProvider(this, GetSendEndpointProvider());

        public IPublishEndpoint PublishEndpoint =>
            _publishEndpoint ??= new PublishEndpoint(new OutboxPublishEndpointProvider(this, GetPublishEndpointProvider()));

        public IScopedClientFactory ClientFactory => _scopedClientFactory ??= GetClientFactory();

        bool WasCommitted()
        {
            return _transactionId != _dbContext.TransactionId;
        }

        async Task<Guid?> StartOutboxTransaction(CancellationToken cancellationToken)
        {
            await _dbContext.BeginTransaction(cancellationToken).ConfigureAwait(false);

            var outboxId = _dbContext.TransactionId ?? throw new ArgumentNullException(nameof(_dbContext.TransactionId));

            var outboxState = new OutboxState
            {
                OutboxId = outboxId,
                Created = DateTime.UtcNow
            };

            await _outboxStates.InsertOne(outboxState, cancellationToken).ConfigureAwait(false);

            return outboxId;
        }

        protected virtual ScopedClientFactory GetClientFactory()
        {
            return new ScopedClientFactory(new ClientFactory(new ScopedClientFactoryContext(_clientFactory, _provider)), null);
        }

        protected virtual IPublishEndpointProvider GetPublishEndpointProvider()
        {
            return _bus;
        }

        protected virtual ISendEndpointProvider GetSendEndpointProvider()
        {
            return _bus;
        }
    }
}
