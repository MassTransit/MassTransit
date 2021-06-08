namespace MassTransit.ExtensionsDependencyInjectionIntegration.ScopeProviders
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Filters;
    using GreenPipes.Specifications;
    using Internals.Extensions;
    using Metadata;
    using Microsoft.Extensions.DependencyInjection;
    using Scoping.Filters;


    public static class DependencyInjectionFilterContextScopeProvider
    {
        public static void AddScopedFilter<TContext, TFilterContext, T>(this IPipeConfigurator<TContext> configurator, Type scopedType,
            IServiceProvider provider)
            where TContext : class, TFilterContext
            where TFilterContext : class, PipeContext
            where T : class
        {
            if (!scopedType.IsGenericType || !scopedType.IsGenericTypeDefinition)
                throw new ArgumentException("The scoped filter must be a generic type definition", nameof(scopedType));

            var filterType = scopedType.MakeGenericType(typeof(T));

            if (!filterType.HasInterface(typeof(IFilter<TFilterContext>)))
                throw new ArgumentException($"The scoped filter must implement {TypeMetadataCache<IFilter<TFilterContext>>.ShortName} ", nameof(scopedType));

            var scopeProviderType = typeof(DependencyInjectionFilterContextScopeProvider<,,>).MakeGenericType(filterType, typeof(TContext), typeof
                (TFilterContext));

            var scopeProvider = (IFilterContextScopeProvider<TContext>)Activator.CreateInstance(scopeProviderType, provider);

            var filter = new ScopedFilter<TContext>(scopeProvider);
            var specification = new FilterPipeSpecification<TContext>(filter);

            configurator.AddPipeSpecification(specification);
        }

        public static void AddScopedFilter<TContext, T>(this IPipeConfigurator<TContext> configurator, Type scopedType,
            IServiceProvider provider)
            where TContext : class, PipeContext
            where T : class
        {
            if (typeof(T).HasInterface<Fault>())
                return;

            if (!scopedType.IsGenericType || !scopedType.IsGenericTypeDefinition)
                throw new ArgumentException("The scoped filter must be a generic type definition", nameof(scopedType));

            var filterType = scopedType.MakeGenericType(typeof(T));

            if (!filterType.HasInterface(typeof(IFilter<TContext>)))
                throw new ArgumentException($"The scoped filter must implement {TypeMetadataCache<IFilter<TContext>>.ShortName} ", nameof(scopedType));

            var scopeProviderType = typeof(DependencyInjectionFilterContextScopeProvider<,>).MakeGenericType(filterType, typeof(TContext));

            var scopeProvider = (IFilterContextScopeProvider<TContext>)Activator.CreateInstance(scopeProviderType, provider);

            var filter = new ScopedFilter<TContext>(scopeProvider);
            var specification = new FilterPipeSpecification<TContext>(filter);

            configurator.AddPipeSpecification(specification);
        }
    }


    public class DependencyInjectionFilterContextScopeProvider<TFilter, TContext> :
        IFilterContextScopeProvider<TContext>
        where TFilter : class, IFilter<TContext>
        where TContext : class, PipeContext
    {
        readonly IServiceProvider _serviceProvider;

        public DependencyInjectionFilterContextScopeProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IFilterContextScope<TContext> Create(TContext context)
        {
            return new DependencyInjectionFilterContextScope(context, _serviceProvider);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("filter", TypeMetadataCache<TFilter>.ShortName);
        }


        class DependencyInjectionFilterContextScope :
            IFilterContextScope<TContext>
        {
            readonly IServiceScope _scope;

            public DependencyInjectionFilterContextScope(TContext context, IServiceProvider serviceProvider)
            {
                Context = context;
                _scope = context.TryGetPayload(out IServiceProvider provider)
                    || context.TryGetPayload(out ConsumeContext consumeContext) && consumeContext.TryGetPayload(out provider)
                        ? new NoopScope(provider)
                        : serviceProvider.CreateScope();
            }

            public ValueTask DisposeAsync()
            {
                _scope.Dispose();

                return default;
            }

            public IFilter<TContext> Filter => ActivatorUtilities.GetServiceOrCreateInstance<TFilter>(_scope.ServiceProvider);

            public TContext Context { get; }


            class NoopScope :
                IServiceScope
            {
                public NoopScope(IServiceProvider serviceProvider)
                {
                    ServiceProvider = serviceProvider;
                }

                public void Dispose()
                {
                }

                public IServiceProvider ServiceProvider { get; }
            }
        }
    }


    public class DependencyInjectionFilterContextScopeProvider<TFilter, TContext, TFilterContext> :
        IFilterContextScopeProvider<TContext>
        where TFilter : class, IFilter<TFilterContext>
        where TContext : class, ConsumeContext
        where TFilterContext : class, ConsumeContext
    {
        readonly IServiceProvider _serviceProvider;

        public DependencyInjectionFilterContextScopeProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IFilterContextScope<TContext> Create(TContext context)
        {
            return new DependencyInjectionFilterContextScope(context, _serviceProvider);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("filter", TypeMetadataCache<TFilter>.ShortName);
        }


        class DependencyInjectionFilterContextScope :
            IFilterContextScope<TContext>
        {
            readonly IServiceScope _scope;

            public DependencyInjectionFilterContextScope(TContext context, IServiceProvider serviceProvider)
            {
                Context = context;
                _scope = context.TryGetPayload(out IServiceProvider provider)
                    || context.TryGetPayload(out ConsumeContext consumeContext) && consumeContext.TryGetPayload(out provider)
                        ? new NoopScope(provider)
                        : serviceProvider.CreateScope();
            }

            public ValueTask DisposeAsync()
            {
                _scope.Dispose();

                return default;
            }

            public IFilter<TContext> Filter
            {
                get
                {
                    var filter = ActivatorUtilities.GetServiceOrCreateInstance<TFilter>(_scope.ServiceProvider);

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
                return context as TFilterContext;
            }


            class NoopScope :
                IServiceScope
            {
                public NoopScope(IServiceProvider serviceProvider)
                {
                    ServiceProvider = serviceProvider;
                }

                public void Dispose()
                {
                }

                public IServiceProvider ServiceProvider { get; }
            }
        }
    }
}
