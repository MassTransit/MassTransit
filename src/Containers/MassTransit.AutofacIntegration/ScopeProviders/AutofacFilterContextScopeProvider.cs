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
    using GreenPipes.Specifications;
    using Internals.Extensions;
    using Metadata;
    using Scoping.Filters;


    public static class AutofacFilterContextScopeProvider
    {
        public static void AddScopedFilter<TContext, TFilterContext, T>(this IPipeConfigurator<TContext> configurator, Type scopedType,
            ILifetimeScopeProvider provider)
            where TContext : class, TFilterContext
            where TFilterContext : class, PipeContext
            where T : class
        {
            if (!scopedType.IsGenericType || !scopedType.IsGenericTypeDefinition)
                throw new ArgumentException("The scoped filter must be a generic type definition", nameof(scopedType));

            var filterType = scopedType.MakeGenericType(typeof(T));

            if (!filterType.HasInterface(typeof(IFilter<TFilterContext>)))
                throw new ArgumentException($"The scoped filter must implement {TypeMetadataCache<IFilter<TFilterContext>>.ShortName} ", nameof(scopedType));

            var scopeProviderType = typeof(AutofacFilterContextScopeProvider<,,>).MakeGenericType(filterType, typeof(TContext), typeof(TFilterContext));

            var scopeProvider = (IFilterContextScopeProvider<TContext>)Activator.CreateInstance(scopeProviderType, provider);

            var filter = new ScopedFilter<TContext>(scopeProvider);
            var specification = new FilterPipeSpecification<TContext>(filter);

            configurator.AddPipeSpecification(specification);
        }

        public static void AddScopedFilter<TContext, T>(this IPipeConfigurator<TContext> configurator, Type scopedType,
            ILifetimeScopeProvider provider)
            where TContext : class, PipeContext
            where T : class
        {
            if (!scopedType.IsGenericType || !scopedType.IsGenericTypeDefinition)
                throw new ArgumentException("The scoped filter must be a generic type definition", nameof(scopedType));

            var filterType = scopedType.MakeGenericType(typeof(T));

            if (!filterType.HasInterface(typeof(IFilter<TContext>)))
                throw new ArgumentException($"The scoped filter must implement {TypeMetadataCache<IFilter<TContext>>.ShortName} ", nameof(scopedType));

            var scopeProviderType = typeof(AutofacFilterContextScopeProvider<,>).MakeGenericType(filterType, typeof(TContext));

            var scopeProvider = (IFilterContextScopeProvider<TContext>)Activator.CreateInstance(scopeProviderType, provider);

            var filter = new ScopedFilter<TContext>(scopeProvider);
            var specification = new FilterPipeSpecification<TContext>(filter);

            configurator.AddPipeSpecification(specification);
        }
    }


    public class AutofacFilterContextScopeProvider<TFilter, TContext> :
        IFilterContextScopeProvider<TContext>
        where TFilter : class, IFilter<TContext>
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

            public IFilter<TContext> Filter => ActivatorUtils.GetOrCreateInstance<TFilter>(_lifetimeScope);

            public TContext Context { get; }


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


    public class AutofacFilterContextScopeProvider<TFilter, TContext, TFilterContext> :
        IFilterContextScopeProvider<TContext>
        where TFilter : class, IFilter<TFilterContext>
        where TContext : class, TFilterContext
        where TFilterContext : class, PipeContext
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

                    return new SplitFilter<TContext, TFilterContext>(filter, ContextProvider, InputContextProvider);
                }
            }

            public TContext Context { get; }

            static TContext ContextProvider(TContext context, TFilterContext splitContext)
            {
                return context;
            }

            static TFilterContext InputContextProvider(TContext context)
            {
                return context;
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
