namespace MassTransit.AutofacIntegration.ScopeProviders
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using Autofac.Core;
    using Autofac.Core.Lifetime;
    using Autofac.Core.Resolving;
    using GreenPipes;
    using GreenPipes.Filters;
    using Metadata;
    using Scoping.Filters;


    public class AutofacConsumeFilterContextScopeProvider<TFilter, TContext, TMessage> :
        IFilterContextScopeProvider<TContext>
        where TFilter : class, IFilter<ConsumeContext<TMessage>>
        where TContext : class, ConsumeContext
        where TMessage : class
    {
        readonly ILifetimeScopeProvider _lifetimeScopeProvider;

        public AutofacConsumeFilterContextScopeProvider(ILifetimeScopeProvider lifetimeScopeProvider)
        {
            _lifetimeScopeProvider = lifetimeScopeProvider;
        }

        public IFilterContextScope<TContext> Create(TContext context)
        {
            var scope = new AutofacFilterContextScope(context, _lifetimeScopeProvider.LifetimeScope);
            return scope;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("filter", TypeMetadataCache<TFilter>.ShortName);
        }


        class AutofacFilterContextScope :
            IFilterContextScope<TContext>
        {
            readonly ILifetimeScope _lifetimeScope;

            public AutofacFilterContextScope(TContext context, ILifetimeScope lifetimeScope)
            {
                Context = context;
                _lifetimeScope = context.TryGetPayload(out ILifetimeScope scope)
                    || context.TryGetPayload(out ConsumeContext consumeContext) && consumeContext.TryGetPayload(out scope)
                        ? new NoopLifetimeScope(scope)
                        : lifetimeScope.BeginLifetimeScope();
            }

            public ValueTask DisposeAsync()
            {
                return _lifetimeScope.DisposeAsync();
            }

            public IFilter<TContext> Filter
            {
                get
                {
                    var filter = ActivatorUtils.GetOrCreateInstance<TFilter>(_lifetimeScope);

                    return new SplitFilter<TContext, ConsumeContext<TMessage>>(filter, ContextProvider, InputContextProvider);
                }
            }

            public TContext Context { get; }

            static TContext ContextProvider(TContext context, ConsumeContext<TMessage> splitContext)
            {
                return context;
            }

            static ConsumeContext<TMessage> InputContextProvider(TContext context)
            {
                return context as ConsumeContext<TMessage>;
            }


            class NoopLifetimeScope :
                ILifetimeScope
            {
                readonly ILifetimeScope _lifetimeScope;

                public NoopLifetimeScope(ILifetimeScope lifetimeScope)
                {
                    _lifetimeScope = lifetimeScope;
                }

                public object ResolveComponent(ResolveRequest request)
                {
                    return _lifetimeScope.ResolveComponent(request);
                }

                public IComponentRegistry ComponentRegistry => _lifetimeScope.ComponentRegistry;

                public void Dispose()
                {
                }

                public ValueTask DisposeAsync()
                {
                    return default;
                }

                public ILifetimeScope BeginLifetimeScope()
                {
                    return _lifetimeScope.BeginLifetimeScope();
                }

                public ILifetimeScope BeginLifetimeScope(object tag)
                {
                    return _lifetimeScope.BeginLifetimeScope(tag);
                }

                public ILifetimeScope BeginLifetimeScope(Action<ContainerBuilder> configurationAction)
                {
                    return _lifetimeScope.BeginLifetimeScope(configurationAction);
                }

                public ILifetimeScope BeginLifetimeScope(object tag, Action<ContainerBuilder> configurationAction)
                {
                    return _lifetimeScope.BeginLifetimeScope(tag, configurationAction);
                }

                public IDisposer Disposer => _lifetimeScope.Disposer;

                public object Tag => _lifetimeScope.Tag;

                public event EventHandler<LifetimeScopeBeginningEventArgs> ChildLifetimeScopeBeginning
                {
                    add => _lifetimeScope.ChildLifetimeScopeBeginning += value;
                    remove => _lifetimeScope.ChildLifetimeScopeBeginning -= value;
                }

                public event EventHandler<LifetimeScopeEndingEventArgs> CurrentScopeEnding
                {
                    add => _lifetimeScope.CurrentScopeEnding += value;
                    remove => _lifetimeScope.CurrentScopeEnding -= value;
                }

                public event EventHandler<ResolveOperationBeginningEventArgs> ResolveOperationBeginning
                {
                    add => _lifetimeScope.ResolveOperationBeginning += value;
                    remove => _lifetimeScope.ResolveOperationBeginning -= value;
                }
            }
        }
    }
}
