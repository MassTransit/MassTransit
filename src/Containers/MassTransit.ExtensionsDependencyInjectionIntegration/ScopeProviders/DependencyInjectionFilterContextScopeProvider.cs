namespace MassTransit.ExtensionsDependencyInjectionIntegration.ScopeProviders
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Scoping.Filters;


    public class DependencyInjectionFilterContextScopeProvider<TFilter, TContext> :
        IFilterContextScopeProvider<TContext>
        where TFilter : IFilter<TContext>
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


        class DependencyInjectionFilterContextScope :
            IFilterContextScope<TContext>
        {
            readonly IServiceScope _scope;

            public DependencyInjectionFilterContextScope(TContext context, IServiceProvider serviceProvider)
            {
                Context = context;
                _scope = context.TryGetPayload(out IServiceProvider provider)
                    ? new NoopScope(provider)
                    : context.TryGetPayload(out ConsumeContext consumeContext) && consumeContext.TryGetPayload(out provider)
                        ? new NoopScope(provider)
                        : serviceProvider.CreateScope();
            }

            public ValueTask DisposeAsync()
            {
                _scope.Dispose();

                return default;
            }

            public IFilter<TContext> Filter => _scope.ServiceProvider.GetRequiredService<TFilter>();

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
