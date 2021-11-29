namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Util;


    public abstract class BaseConsumeScopeProvider
    {
        readonly IServiceProvider _serviceProvider;

        protected BaseConsumeScopeProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected ValueTask<TScopeContext> GetScopeContext<TScopeContext, TPipeContext>(TPipeContext context,
            Func<TPipeContext, IServiceScope, TScopeContext> existingScopeContextFactory,
            Func<TPipeContext, IServiceScope, TScopeContext> createdScopeContextFactory,
            Func<TPipeContext, IServiceScope, IScopeServiceProvider, TPipeContext> pipeContextFactory)
            where TPipeContext : ConsumeContext
        {
            if (context.TryGetPayload<IServiceScope>(out var existingServiceScope))
            {
                existingServiceScope.SetCurrentConsumeContext(context);

                return new ValueTask<TScopeContext>(existingScopeContextFactory(context, existingServiceScope));
            }

            var serviceProvider = context.GetPayload(_serviceProvider);

            var serviceScope = serviceProvider.CreateScope();
            try
            {
                var scopeServiceProvider = new DependencyInjectionScopeServiceProvider(serviceScope.ServiceProvider);

                var scopeContext = pipeContextFactory(context, serviceScope, scopeServiceProvider);

                serviceScope.SetCurrentConsumeContext(scopeContext);

                return new ValueTask<TScopeContext>(createdScopeContextFactory(scopeContext, serviceScope));
            }
            catch (Exception ex)
            {
                if (serviceScope is IAsyncDisposable asyncDisposable)
                    return ex.DisposeAsync<TScopeContext>(() => asyncDisposable.DisposeAsync());

                serviceScope.Dispose();
                throw;
            }
        }
    }
}
