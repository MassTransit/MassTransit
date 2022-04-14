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
        public ScopedBusContextProvider(TBus bus, Bind<TBus, IClientFactory> clientFactory, ScopedConsumeContextProvider consumeContextProvider,
            IServiceProvider provider)
        {
            if (consumeContextProvider.HasContext)
                Context = new ConsumeContextScopedBusContext(consumeContextProvider.GetContext(), clientFactory.Value);
            else
                Context = new BusScopedBusContext<TBus>(bus, clientFactory.Value, provider);
        }

        public ScopedBusContext Context { get; }
    }
}
