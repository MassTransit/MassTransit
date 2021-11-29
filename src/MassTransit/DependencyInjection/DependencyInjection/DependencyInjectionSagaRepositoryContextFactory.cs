namespace MassTransit.DependencyInjection
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Microsoft.Extensions.DependencyInjection;
    using Saga;


    public class DependencyInjectionSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly IServiceProvider _serviceProvider;

        public DependencyInjectionSagaRepositoryContextFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        void IProbeSite.Probe(ProbeContext context)
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

        public async Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken)
            where T : class
        {
            using var serviceScope = _serviceProvider.CreateScope();

            var factory = serviceScope.ServiceProvider.GetRequiredService<ISagaRepositoryContextFactory<TSaga>>();

            return await factory.Execute(asyncMethod, cancellationToken).ConfigureAwait(false);
        }

        Task Send<T>(ConsumeContext<T> context, Func<ConsumeContext<T>, ISagaRepositoryContextFactory<TSaga>, Task> send)
            where T : class
        {
            var serviceProvider = context.GetPayload(_serviceProvider);

            if (context.TryGetPayload<IServiceScope>(out var existingScope))
            {
                existingScope.SetCurrentConsumeContext(context);

                context.GetOrAddPayload(() => existingScope.ServiceProvider.GetService<IStateMachineActivityFactory>()
                    ?? DependencyInjectionStateMachineActivityFactory.Instance);

                var factory = existingScope.ServiceProvider.GetRequiredService<ISagaRepositoryContextFactory<TSaga>>();

                return send(context, factory);
            }

            async Task CreateScope()
            {
                var serviceScope = serviceProvider.CreateScope();
                try
                {
                    var scopeServiceProvider = new DependencyInjectionScopeServiceProvider(serviceScope.ServiceProvider);

                    var scopeContext = new ConsumeContextScope<T>(context, serviceScope, serviceScope.ServiceProvider, scopeServiceProvider);

                    serviceScope.SetCurrentConsumeContext(scopeContext);

                    var activityFactory = serviceScope.ServiceProvider.GetService<IStateMachineActivityFactory>()
                        ?? DependencyInjectionStateMachineActivityFactory.Instance;

                    var consumeContextScope = new ConsumeContextScope<T>(scopeContext, activityFactory);

                    var factory = serviceScope.ServiceProvider.GetRequiredService<ISagaRepositoryContextFactory<TSaga>>();

                    await send(consumeContextScope, factory).ConfigureAwait(false);
                }
                finally
                {
                    if (serviceScope is IAsyncDisposable asyncDisposable)
                        await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                    else
                        serviceScope.Dispose();
                }
            }

            return CreateScope();
        }
    }
}
