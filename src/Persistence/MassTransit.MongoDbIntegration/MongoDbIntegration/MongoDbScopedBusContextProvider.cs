#nullable enable
namespace MassTransit.MongoDbIntegration
{
    using System;
    using DependencyInjection;
    using Middleware.Outbox;


    public class MongoDbScopedBusContextProvider<TBus> :
        IScopedBusContextProvider<TBus>,
        IDisposable
        where TBus : class, IBus
    {
        public MongoDbScopedBusContextProvider(TBus bus, MongoDbContext dbContext, IBusOutboxNotification notification,
            Bind<TBus, IClientFactory> clientFactory, ScopedConsumeContextProvider consumeContextProvider, IServiceProvider provider)
        {
            if (consumeContextProvider.HasContext)
                Context = new ConsumeContextScopedBusContext(consumeContextProvider.GetContext(), clientFactory.Value);
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
}
