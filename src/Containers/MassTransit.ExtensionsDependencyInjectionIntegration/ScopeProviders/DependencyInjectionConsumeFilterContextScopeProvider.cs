namespace MassTransit.ExtensionsDependencyInjectionIntegration.ScopeProviders
{
    using System;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Filters;
    using Metadata;
    using Microsoft.Extensions.DependencyInjection;
    using Scoping.Filters;


    public class DependencyInjectionConsumeFilterContextScopeProvider<TFilter, TContext, TMessage> :
        IFilterContextScopeProvider<TContext>
        where TFilter : class, IFilter<ConsumeContext<TMessage>>
        where TContext : class, ConsumeContext
        where TMessage : class
    {
        readonly IServiceProvider _serviceProvider;

        public DependencyInjectionConsumeFilterContextScopeProvider(IServiceProvider serviceProvider)
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
