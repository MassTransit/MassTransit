namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;
    using Context;
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
            Func<TPipeContext, IServiceScope, IServiceProvider, TPipeContext> pipeContextFactory)
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
                var scopeContext = pipeContextFactory(context, serviceScope, serviceScope.ServiceProvider);

                if (scopeContext.TryGetPayload(out MessageSchedulerContext schedulerContext))
                {
                    scopeContext.AddOrUpdatePayload<MessageSchedulerContext>(
                        () => new ConsumeMessageSchedulerContext(scopeContext, schedulerContext.SchedulerFactory),
                        existing => new ConsumeMessageSchedulerContext(scopeContext, existing.SchedulerFactory));
                }

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
