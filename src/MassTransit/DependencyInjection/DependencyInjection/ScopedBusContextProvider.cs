namespace MassTransit.DependencyInjection
{
    using System;


    /// <summary>
    /// Captures the bus context for the current scope as a scoped provider, so that it can be resolved
    /// by components at runtime (since MS DI doesn't support runtime configuration of scopes)
    /// </summary>
    public class ScopedBusContextProvider<TBus> :
        IScopedBusContextProvider<TBus>
        where TBus : class, IBus
    {
        public ScopedBusContextProvider(TBus bus, Bind<TBus, IClientFactory> clientFactory,
            Bind<TBus, IScopedConsumeContextProvider> consumeContextProvider,
            IScopedConsumeContextProvider globalConsumeContextProvider,
            IServiceProvider provider)
        {
            if (consumeContextProvider.Value.HasContext)
                Context = new ConsumeContextScopedBusContext(consumeContextProvider.Value.GetContext(), clientFactory.Value);
            else if (globalConsumeContextProvider.HasContext)
                Context = new ConsumeContextScopedBusContext<TBus>(bus, globalConsumeContextProvider.GetContext(), clientFactory.Value, provider);
            else
                Context = new BusScopedBusContext<TBus>(bus, clientFactory.Value, provider);
        }

        public ScopedBusContext Context { get; }
    }
}
