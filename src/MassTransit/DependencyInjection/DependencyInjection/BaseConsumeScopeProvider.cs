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
        protected readonly ISetScopedConsumeContext SetScopedConsumeContext;

        protected BaseConsumeScopeProvider(IRegistrationContext context)
            : this(context, context as ISetScopedConsumeContext ?? throw new ArgumentException(nameof(context)))
        {
        }

        protected BaseConsumeScopeProvider(IServiceProvider serviceProvider, ISetScopedConsumeContext setScopedConsumeContext)
        {
            _serviceProvider = serviceProvider;
            SetScopedConsumeContext = setScopedConsumeContext;
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
                    SetScopedConsumeContext.PushContext(existingServiceScope, context)));
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

                return new ValueTask<TScopeContext>(createdScopeContextFactory(scopeContext, serviceScope,
                    SetScopedConsumeContext.PushContext(serviceScope, scopeContext)));
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
