namespace MassTransit.ExtensionsDependencyInjectionIntegration.ScopeProviders
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Automatonymous;
    using Context;
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Registration;
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
            if (!context.TryGetPayload(out IServiceProvider serviceProvider))
                serviceProvider = _serviceProvider;

            if (context.TryGetPayload<IServiceScope>(out var existingScope))
            {
                existingScope.UpdateScope(context);

                context.GetOrAddPayload(() => existingScope.ServiceProvider.GetService<IStateMachineActivityFactory>()
                    ?? DependencyInjectionStateMachineActivityFactory.Instance);

                var factory = existingScope.ServiceProvider.GetRequiredService<ISagaRepositoryContextFactory<TSaga>>();

                return send(context, factory);
            }

            async Task CreateScope()
            {
                using var serviceScope = serviceProvider.CreateScope();

                var scopeServiceProvider = new DependencyInjectionScopeServiceProvider(serviceScope.ServiceProvider);

                var scopeContext = new ConsumeContextScope<T>(context, serviceScope, serviceScope.ServiceProvider, scopeServiceProvider);

                serviceScope.UpdateScope(scopeContext);

                var activityFactory = serviceScope.ServiceProvider.GetService<IStateMachineActivityFactory>()
                    ?? DependencyInjectionStateMachineActivityFactory.Instance;

                var consumeContextScope = new ConsumeContextScope<T>(scopeContext, activityFactory);

                var factory = serviceScope.ServiceProvider.GetRequiredService<ISagaRepositoryContextFactory<TSaga>>();

                await send(consumeContextScope, factory).ConfigureAwait(false);
            }

            return CreateScope();
        }
    }
}
