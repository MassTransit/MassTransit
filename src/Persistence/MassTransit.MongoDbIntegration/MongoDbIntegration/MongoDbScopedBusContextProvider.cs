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
}
