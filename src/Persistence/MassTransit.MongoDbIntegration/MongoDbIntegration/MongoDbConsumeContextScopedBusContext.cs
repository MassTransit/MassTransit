#nullable enable
namespace MassTransit.MongoDbIntegration;

using System;
using Clients;
using DependencyInjection;
using Middleware.Outbox;


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
