namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using DependencyInjection;
    using Microsoft.EntityFrameworkCore;
    using Middleware.Outbox;


    public class EntityFrameworkScopedBusContextProvider<TBus, TDbContext> :
        IScopedBusContextProvider<TBus>,
        IDisposable
        where TBus : class, IBus
        where TDbContext : DbContext
    {
        public EntityFrameworkScopedBusContextProvider(TBus bus, TDbContext dbContext, IBusOutboxNotification notification,
            Bind<TBus, IClientFactory> clientFactory,
            ScopedConsumeContextProvider consumeContextProvider, IServiceProvider provider)
        {
            if (consumeContextProvider.HasContext)
                Context = new ConsumeContextScopedBusContext(consumeContextProvider.GetContext(), clientFactory.Value);
            else
                Context = new EntityFrameworkScopedBusContext<TBus, TDbContext>(bus, dbContext, notification, clientFactory.Value, provider);
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
}
