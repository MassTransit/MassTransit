namespace MassTransit.AutofacIntegration.ScopeProviders
{
    using System;
    using System.Threading.Tasks;
    using Autofac;
    using Autofac.Core;
    using Autofac.Core.Lifetime;
    using Autofac.Core.Resolving;
    using GreenPipes;
    using Scoping.Filters;


    public class AutofacFilterContextScopeProvider<TFilter, TContext> :
        IFilterContextScopeProvider<TContext>
        where TFilter : IFilter<TContext>
        where TContext : class, PipeContext
    {
        readonly ILifetimeScopeProvider _lifetimeScopeProvider;

        public AutofacFilterContextScopeProvider(ILifetimeScopeProvider lifetimeScopeProvider)
        {
            _lifetimeScopeProvider = lifetimeScopeProvider;
        }

        public IFilterContextScope<TContext> Create(TContext context)
        {
            var scope = new AutofacFilterContextScope(context, _lifetimeScopeProvider.LifetimeScope);
            return scope;
        }


        class AutofacFilterContextScope :
            IFilterContextScope<TContext>
        {
            readonly ILifetimeScope _lifetimeScope;

            public AutofacFilterContextScope(TContext context, ILifetimeScope lifetimeScope)
            {
                Context = context;
                _lifetimeScope = context.TryGetPayload(out ILifetimeScope scope) ? new NoopLifetimeScope(scope) : lifetimeScope.BeginLifetimeScope();
            }

            public void Dispose()
            {
                _lifetimeScope.Dispose();
            }

            public IFilter<TContext> Filter => _lifetimeScope.Resolve<TFilter>();

            public TContext Context { get; }


            class NoopLifetimeScope :
                ILifetimeScope
            {
                readonly ILifetimeScope _lifetimeScope;

                public NoopLifetimeScope(ILifetimeScope lifetimeScope)
                {
                    _lifetimeScope = lifetimeScope;
                }

                public object ResolveComponent(ResolveRequest request) => _lifetimeScope.ResolveComponent(request);

                public IComponentRegistry ComponentRegistry => _lifetimeScope.ComponentRegistry;

                public void Dispose()
                {
                }

                public ValueTask DisposeAsync() => default;

                public ILifetimeScope BeginLifetimeScope() => _lifetimeScope.BeginLifetimeScope();

                public ILifetimeScope BeginLifetimeScope(object tag) => _lifetimeScope.BeginLifetimeScope(tag);

                public ILifetimeScope BeginLifetimeScope(Action<ContainerBuilder> configurationAction) =>
                    _lifetimeScope.BeginLifetimeScope(configurationAction);

                public ILifetimeScope BeginLifetimeScope(object tag, Action<ContainerBuilder> configurationAction) =>
                    _lifetimeScope.BeginLifetimeScope(tag, configurationAction);

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
