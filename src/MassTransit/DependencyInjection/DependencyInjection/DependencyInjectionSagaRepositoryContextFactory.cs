namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Microsoft.Extensions.DependencyInjection;
    using Saga;


    public class DependencyInjectionSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly IServiceProvider _serviceProvider;
        readonly ISetScopedConsumeContext _setter;

        public DependencyInjectionSagaRepositoryContextFactory(IRegistrationContext context)
            : this(context, context as ISetScopedConsumeContext ?? throw new ArgumentException(nameof(context)))
        {
        }

        public DependencyInjectionSagaRepositoryContextFactory(IServiceProvider serviceProvider, ISetScopedConsumeContext setter)
        {
            _serviceProvider = serviceProvider;
            _setter = setter;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "dependencyInjection");
        }

        public Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class
        {
            return Send(context, (consumeContext, factory) => factory.Send(consumeContext, next));
        }

        public Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class
        {
            return Send(context, (consumeContext, factory) => factory.SendQuery(consumeContext, query, next));
        }

        async Task Send<T>(ConsumeContext<T> context, Func<ConsumeContext<T>, ISagaRepositoryContextFactory<TSaga>, Task> send)
            where T : class
        {
            var serviceProvider = context.GetPayload(_serviceProvider);

            IDisposable disposable = null;

            if (context.TryGetPayload<IServiceScope>(out var existingScope))
            {
                disposable = _setter.PushContext(existingScope, context);

                try
                {
                    var factory = existingScope.ServiceProvider.GetRequiredService<ISagaRepositoryContextFactory<TSaga>>();
                    await send(context, factory).ConfigureAwait(false);
                }
                finally
                {
                    disposable.Dispose();
                }

                return;
            }

            var serviceScope = serviceProvider.CreateScope();
            try
            {
                var scopeContext = new ConsumeContextScope<T>(context, serviceScope, serviceScope.ServiceProvider);

                if (scopeContext.TryGetPayload(out MessageSchedulerContext schedulerContext))
                {
                    scopeContext.AddOrUpdatePayload<MessageSchedulerContext>(
                        () => new ConsumeMessageSchedulerContext(scopeContext, schedulerContext.SchedulerFactory),
                        existing => new ConsumeMessageSchedulerContext(scopeContext, existing.SchedulerFactory));
                }

                disposable = _setter.PushContext(serviceScope, scopeContext);

                var consumeContextScope = new ConsumeContextScope<T>(scopeContext);

                var factory = serviceScope.ServiceProvider.GetRequiredService<ISagaRepositoryContextFactory<TSaga>>();

                await send(consumeContextScope, factory).ConfigureAwait(false);
            }
            finally
            {
                disposable?.Dispose();

                if (serviceScope is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                else
                    serviceScope.Dispose();
            }
        }
    }
}
