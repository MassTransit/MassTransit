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
            Func<TPipeContext, IServiceScope, IDisposable, TScopeContext> existingScopeContextFactory,
            Func<TPipeContext, IServiceScope, IDisposable, TScopeContext> createdScopeContextFactory,
            Func<TPipeContext, IServiceScope, IServiceProvider, TPipeContext> pipeContextFactory)
            where TPipeContext : ConsumeContext
        {
            if (context.TryGetPayload<IServiceScope>(out var existingServiceScope))
            {
                return new ValueTask<TScopeContext>(existingScopeContextFactory(context, existingServiceScope,
                    existingServiceScope.SetCurrentConsumeContext(context)));
            }

            var serviceProvider = context.GetPayload(_serviceProvider);

            var serviceScope = serviceProvider.CreateAsyncScope();
            try
            {
                var scopeContext = pipeContextFactory(context, serviceScope, serviceScope.ServiceProvider);

                if (scopeContext.TryGetPayload(out MessageSchedulerContext schedulerContext))
                {
                    scopeContext.AddOrUpdatePayload<MessageSchedulerContext>(
                        () => new ConsumeMessageSchedulerContext(scopeContext, schedulerContext.SchedulerFactory),
                        existing => new ConsumeMessageSchedulerContext(scopeContext, existing.SchedulerFactory));
                }

                return new ValueTask<TScopeContext>(createdScopeContextFactory(scopeContext, serviceScope,
                    serviceScope.SetCurrentConsumeContext(scopeContext)));
            }
            catch (Exception ex)
            {
                return ex.DisposeAsync<TScopeContext>(() => serviceScope.DisposeAsync());
            }
        }
    }
}
