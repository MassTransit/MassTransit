namespace MassTransit.DependencyInjection
{
    using System;
    using Transactions;


    public class TransactionalScopedBusContextProvider<TBus> :
        IScopedBusContextProvider<TBus>
        where TBus : class, IBus
    {
        public TransactionalScopedBusContextProvider(ITransactionalBus bus, Bind<TBus, IClientFactory> clientFactory,
            Bind<TBus, IScopedConsumeContextProvider> consumeContextProvider, IScopedConsumeContextProvider globalConsumeContextProvider,
            IServiceProvider provider)
        {
            if (consumeContextProvider.Value.HasContext)
                Context = new ConsumeContextScopedBusContext(consumeContextProvider.Value.GetContext(), clientFactory.Value);
            else if (globalConsumeContextProvider.HasContext)
                Context = new ConsumeContextScopedBusContext<IBus>(bus, globalConsumeContextProvider.GetContext(), clientFactory.Value, provider);
            else
                Context = new BusScopedBusContext<IBus>(bus, clientFactory.Value, provider);
        }

        public ScopedBusContext Context { get; }
    }
}
