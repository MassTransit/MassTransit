#nullable enable
namespace MassTransit.MongoDbIntegration
{
    using System;
    using DependencyInjection;


    public class MongoDbScopedBusContextProvider<TBus> :
        IScopedBusContextProvider<TBus>
        where TBus : class, IBus
    {
        public MongoDbScopedBusContextProvider(TBus bus, MongoDbContext dbContext, Bind<TBus, IClientFactory> clientFactory,
            ScopedConsumeContextProvider consumeContextProvider, IServiceProvider provider)
        {
            if (consumeContextProvider.HasContext)
                Context = new ConsumeContextScopedBusContext(consumeContextProvider.GetContext(), clientFactory.Value);
            else
                Context = new MongoDbScopedBusContext<TBus>(bus, dbContext, clientFactory.Value, provider);
        }

        public ScopedBusContext Context { get; }
    }
}
