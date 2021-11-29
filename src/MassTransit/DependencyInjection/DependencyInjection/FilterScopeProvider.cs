namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;


    /// <summary>
    /// Used by Send/Publish filters to send within either a scoped endpoint/request client context or within the consume context
    /// currently active
    /// </summary>
    /// <typeparam name="TFilter"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public class FilterScopeProvider<TFilter, TContext> :
        IFilterScopeProvider<TContext>
        where TFilter : class, IFilter<TContext>
        where TContext : class, PipeContext
    {
        readonly IServiceProvider _serviceProvider;

        public FilterScopeProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IFilterScopeContext<TContext> Create(TContext context)
        {
            return new DependencyInjectionFilterScopeContext(context, _serviceProvider);
        }

        public void Probe(ProbeContext context)
        {
            context.Add("filter", TypeCache<TFilter>.ShortName);
        }


        class DependencyInjectionFilterScopeContext :
            IFilterScopeContext<TContext>
        {
            readonly IServiceScope _scope;
            TFilter _filter;

            public DependencyInjectionFilterScopeContext(TContext context, IServiceProvider serviceProvider)
            {
                Context = context;
                _scope = context.TryGetPayload(out IServiceProvider provider)
                    || context.TryGetPayload(out ConsumeContext consumeContext) && consumeContext.TryGetPayload(out provider)
                        ? new NoopScope(provider)
                        : serviceProvider.CreateScope();
            }

            public ValueTask DisposeAsync()
            {
                if (_scope is IAsyncDisposable asyncDisposable)
                    return asyncDisposable.DisposeAsync();

                _scope.Dispose();

                return default;
            }

            public IFilter<TContext> Filter => _filter ??= ActivatorUtilities.GetServiceOrCreateInstance<TFilter>(_scope.ServiceProvider);

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
}
