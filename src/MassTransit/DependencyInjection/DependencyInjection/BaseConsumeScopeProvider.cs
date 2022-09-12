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

                if (scopeContext.TryGetPayload(out MessageSchedulerContext schedulerContext))
                {
                    context.AddOrUpdatePayload<MessageSchedulerContext>(
                        () => new ConsumeMessageSchedulerContext(scopeContext, schedulerContext.SchedulerFactory, context.ReceiveContext.InputAddress),
                        existing => new ConsumeMessageSchedulerContext(scopeContext, existing.SchedulerFactory, context.ReceiveContext.InputAddress));
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
