#nullable enable
namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Threading.Tasks;
    using Clients;
    using DependencyInjection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Middleware;
    using Middleware.Outbox;
    using Serialization;
    using Transports;


    public class EntityFrameworkScopedBusContext<TBus, TDbContext> :
        ScopedBusContext,
        OutboxSendContext,
        IDisposable
        where TBus : class, IBus
        where TDbContext : DbContext
    {
        readonly TBus _bus;
        readonly IClientFactory _clientFactory;
        readonly TDbContext _dbContext;
        readonly IBusOutboxNotification _notification;
        readonly DbSet<OutboxMessage> _outboxMessageSet;
        readonly DbSet<OutboxState> _outboxStateSet;
        readonly IServiceProvider _provider;
        Guid _outboxId;
        EntityEntry<OutboxState>? _outboxState;
        IPublishEndpoint? _publishEndpoint;
        IScopedClientFactory? _scopedClientFactory;
        ISendEndpointProvider? _sendEndpointProvider;

        public EntityFrameworkScopedBusContext(TBus bus, TDbContext dbContext, IBusOutboxNotification notification, IClientFactory clientFactory,
            IServiceProvider provider)
        {
            _bus = bus;
            _dbContext = dbContext;
            _notification = notification;
            _clientFactory = clientFactory;
            _provider = provider;

            _outboxId = NewId.NextGuid();

            _outboxMessageSet = dbContext.Set<OutboxMessage>();
            _outboxStateSet = dbContext.Set<OutboxState>();
        }

        public void Dispose()
        {
            if (WasCommitted())
                _notification.Delivered();

            _outboxState = null;
        }

        public Task AddSend<T>(SendContext<T> context)
            where T : class
        {
            lock (_outboxStateSet)
            {
                if (_outboxState == null || WasCommitted())
                {
                    if (WasCommitted())
                    {
                        _notification.Delivered();
                        _outboxId = NewId.NextGuid();
                        _outboxState = null;
                    }

                    _outboxState ??= _dbContext.Add(new OutboxState
                    {
                        OutboxId = _outboxId,
                        Created = DateTime.UtcNow
                    });
                }
            }

            return _outboxMessageSet.AddSend(context, SystemTextJsonMessageSerializer.Instance, outboxId: _outboxId);
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
            return _outboxState?.State == EntityState.Unchanged;
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
