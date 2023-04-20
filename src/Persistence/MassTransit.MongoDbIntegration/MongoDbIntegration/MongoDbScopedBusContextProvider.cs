#nullable enable
namespace MassTransit.MongoDbIntegration
{
    using System;
    using Clients;
    using DependencyInjection;
    using Middleware.Outbox;


    public class MongoDbScopedBusContextProvider<TBus> :
        IScopedBusContextProvider<TBus>,
        IDisposable
        where TBus : class, IBus
    {
        public MongoDbScopedBusContextProvider(TBus bus, MongoDbContext dbContext, IBusOutboxNotification notification,
            Bind<TBus, IClientFactory> clientFactory, Bind<TBus, IScopedConsumeContextProvider> consumeContextProvider,
            IScopedConsumeContextProvider globalConsumeContextProvider, IServiceProvider provider)
        {
            if (consumeContextProvider.Value.HasContext)
                Context = new ConsumeContextScopedBusContext(consumeContextProvider.Value.GetContext(), clientFactory.Value);
            else if (globalConsumeContextProvider.HasContext)
            {
                Context = new MongoDbConsumeContextScopedBusContext<TBus>(bus, dbContext, notification, clientFactory.Value, provider,
                    globalConsumeContextProvider.GetContext());
            }
            else
                Context = new MongoDbScopedBusContext<TBus>(bus, dbContext, notification, clientFactory.Value, provider);
        }

        public void Dispose()
        {
            switch (Context)
            {
                case IDisposable disposable:
                    disposable.Dispose();
                    return;
            }
        }

        public ScopedBusContext Context { get; }
    }


    public class MongoDbConsumeContextScopedBusContext<TBus> :
        MongoDbScopedBusContext<TBus>
        where TBus : class, IBus
    {
        readonly TBus _bus;
        readonly IClientFactory _clientFactory;
        readonly ConsumeContext _consumeContext;
        readonly IServiceProvider _provider;

        public MongoDbConsumeContextScopedBusContext(TBus bus, MongoDbContext dbContext, IBusOutboxNotification notification, IClientFactory clientFactory,
            IServiceProvider provider, ConsumeContext consumeContext)
            : base(bus, dbContext, notification, clientFactory, provider)
        {
            _bus = bus;
            _clientFactory = clientFactory;
            _provider = provider;
            _consumeContext = consumeContext;
        }

        protected override IPublishEndpointProvider GetPublishEndpointProvider()
        {
            return new ScopedConsumePublishEndpointProvider(_bus, _consumeContext, _provider);
        }

        protected override ISendEndpointProvider GetSendEndpointProvider()
        {
            return new ScopedConsumeSendEndpointProvider(_bus, _consumeContext, _provider);
        }

        protected override ScopedClientFactory GetClientFactory()
        {
            return new ScopedClientFactory(new ClientFactory(new ScopedClientFactoryContext(_clientFactory, _provider)), _consumeContext);
        }
    }
}
