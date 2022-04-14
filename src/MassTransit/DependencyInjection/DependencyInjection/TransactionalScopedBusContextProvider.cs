namespace MassTransit.DependencyInjection
{
    using System;
    using Transactions;


    public class TransactionalScopedBusContextProvider<TBus> :
        IScopedBusContextProvider<TBus>
        where TBus : class, IBus
    {
        public TransactionalScopedBusContextProvider(ITransactionalBus bus, Bind<TBus, IClientFactory> clientFactory,
            ScopedConsumeContextProvider consumeContextProvider, IServiceProvider provider)
        {
            if (consumeContextProvider.HasContext)
                Context = new ConsumeContextScopedBusContext(consumeContextProvider.GetContext(), clientFactory.Value);
            else
                Context = new BusScopedBusContext<IBus>(bus, clientFactory.Value, provider);
        }

        public ScopedBusContext Context { get; }
    }
}
