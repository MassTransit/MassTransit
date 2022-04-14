namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using DependencyInjection;
    using Microsoft.EntityFrameworkCore;


    public class EntityFrameworkScopedBusContextProvider<TBus, TDbContext> :
        IScopedBusContextProvider<TBus>
        where TBus : class, IBus
        where TDbContext : DbContext
    {
        public EntityFrameworkScopedBusContextProvider(TBus bus, TDbContext dbContext, Bind<TBus, IClientFactory> clientFactory,
            ScopedConsumeContextProvider consumeContextProvider, IServiceProvider provider)
        {
            if (consumeContextProvider.HasContext)
                Context = new ConsumeContextScopedBusContext(consumeContextProvider.GetContext(), clientFactory.Value);
            else
                Context = new EntityFrameworkScopedBusContext<TBus, TDbContext>(bus, dbContext, clientFactory.Value, provider);
        }

        public ScopedBusContext Context { get; }
    }
}
