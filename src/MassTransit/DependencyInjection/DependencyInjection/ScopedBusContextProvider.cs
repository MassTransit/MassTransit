namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading;


    /// <summary>
    /// Captures the bus context for the current scope as a scoped provider, so that it can be resolved
    /// by components at runtime (since MS DI doesn't support runtime configuration of scopes)
    /// </summary>
    public class ScopedBusContextProvider<TBus> :
        IScopedBusContextProvider<TBus>
        where TBus : class, IBus
    {
        public ScopedBusContextProvider(TBus bus, Bind<TBus, IClientFactory> clientFactory,
            Bind<TBus, IScopedConsumeContextProvider> busConsumeContextProvider,
            IScopedBusContextProvider scopedBusContextProvider,
            IScopedConsumeContextProvider globalConsumeContextProvider,
            IServiceProvider provider)
        {
            if (busConsumeContextProvider.Value.HasContext)
                Context = new ConsumeContextScopedBusContext(busConsumeContextProvider.Value.GetContext(), clientFactory.Value);
            else if (scopedBusContextProvider.HasContext)
                Context = new BusScopedBusContext(scopedBusContextProvider.GetContext(), clientFactory.Value, provider);
            else if (globalConsumeContextProvider.HasContext)
                Context = new ConsumeContextScopedBusContext<TBus>(bus, globalConsumeContextProvider.GetContext(), clientFactory.Value, provider);
            else
                Context = new BusScopedBusContext<TBus>(bus, clientFactory.Value, provider);
        }

        public ScopedBusContext Context { get; }
    }


    public class ScopedBusContextProvider :
        IScopedBusContextProvider
    {
        ScopedBusContext _context;

        public bool HasContext => _context != null;

        public virtual IDisposable PushContext(ScopedBusContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            lock (this)
            {
                var originalContext = _context;

                _context = context;

                return new PushedContext(this, context, originalContext);
            }
        }

        public ScopedBusContext GetContext()
        {
            return _context;
        }

        void PopContext(ScopedBusContext context, ScopedBusContext originalContext)
        {
            Interlocked.CompareExchange(ref _context, originalContext, context);
        }


        class PushedContext :
            IDisposable
        {
            readonly ScopedBusContext _context;
            readonly ScopedBusContext _originalContext;
            readonly ScopedBusContextProvider _provider;

            public PushedContext(ScopedBusContextProvider provider, ScopedBusContext context, ScopedBusContext originalContext)
            {
                _provider = provider;
                _context = context;
                _originalContext = originalContext;
            }

            public void Dispose()
            {
                _provider.PopContext(_context, _originalContext);
            }
        }
    }
}
