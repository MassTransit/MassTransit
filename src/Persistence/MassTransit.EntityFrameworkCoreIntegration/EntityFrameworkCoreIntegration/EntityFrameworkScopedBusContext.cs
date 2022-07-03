#nullable enable
namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Threading.Tasks;
    using Clients;
    using DependencyInjection;
    using Microsoft.EntityFrameworkCore;
    using Middleware;
    using Middleware.Outbox;
    using Serialization;
    using Transports;


    public class EntityFrameworkScopedBusContext<TBus, TDbContext> :
        ScopedBusContext,
        OutboxSendContext
        where TBus : class, IBus
        where TDbContext : DbContext
    {
        readonly TBus _bus;
        readonly IClientFactory _clientFactory;
        readonly TDbContext _dbContext;
        readonly object _lock = new object();
        readonly Guid _outboxId;
        readonly IServiceProvider _provider;
        OutboxState? _outboxState;

        IPublishEndpoint? _publishEndpoint;
        IScopedClientFactory? _scopedClientFactory;
        ISendEndpointProvider? _sendEndpointProvider;

        public EntityFrameworkScopedBusContext(TBus bus, TDbContext dbContext, IClientFactory clientFactory, IServiceProvider provider)
        {
            _bus = bus;
            _dbContext = dbContext;
            _clientFactory = clientFactory;
            _provider = provider;

            _outboxId = NewId.NextGuid();
        }

        public Task AddSend<T>(SendContext<T> context)
            where T : class
        {
            if (_outboxState == null)
            {
                lock (_lock)
                {
                    if (_outboxState == null)
                    {
                        _outboxState = new OutboxState
                        {
                            OutboxId = _outboxId,
                            Created = DateTime.UtcNow
                        };
                        _dbContext.Add(_outboxState);
                    }
                }
            }

            return _dbContext.Set<OutboxMessage>().AddSend(context, SystemTextJsonMessageSerializer.Instance, outboxId: _outboxId);
        }

        public ISendEndpointProvider SendEndpointProvider
        {
            get { return _sendEndpointProvider ??= new ScopedSendEndpointProvider<IServiceProvider>(new OutboxSendEndpointProvider(this, _bus), _provider); }
        }

        public IPublishEndpoint PublishEndpoint
        {
            get
            {
                return _publishEndpoint ??= new PublishEndpoint(new ScopedPublishEndpointProvider<IServiceProvider>(
                    new OutboxPublishEndpointProvider(this, _bus), _provider));
            }
        }

        public IScopedClientFactory ClientFactory
        {
            get
            {
                return _scopedClientFactory ??=
                    new ScopedClientFactory(new ClientFactory(new ScopedClientFactoryContext<IServiceProvider>(_clientFactory, _provider)), null);
            }
        }
    }
}
