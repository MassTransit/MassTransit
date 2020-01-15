namespace MassTransit.ExtensionsDependencyInjectionIntegration.ScopeProviders
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Automatonymous;
    using Context;
    using GreenPipes;
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
            if (!context.TryGetPayload(out IServiceProvider serviceProvider))
                serviceProvider = _serviceProvider;

            if (context.TryGetPayload<IServiceScope>(out var existingScope))
            {
                existingScope.UpdateScope(context);

                context.GetOrAddPayload(() => existingScope.ServiceProvider.GetService<IStateMachineActivityFactory>()
                    ?? DependencyInjectionStateMachineActivityFactory.Instance);

                var factory = existingScope.ServiceProvider.GetRequiredService<ISagaRepositoryContextFactory<TSaga>>();

                return factory.Send(context, next);
            }

            async Task CreateScope()
            {
                using var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

                serviceScope.UpdateScope(context);

                var scopeServiceProvider = serviceScope.ServiceProvider;

                var activityFactory = scopeServiceProvider.GetService<IStateMachineActivityFactory>()
                    ?? DependencyInjectionStateMachineActivityFactory.Instance;

                var consumeContextScope = new ConsumeContextScope<T>(context, serviceScope, scopeServiceProvider, activityFactory);

                var factory = scopeServiceProvider.GetRequiredService<ISagaRepositoryContextFactory<TSaga>>();

                await factory.Send(consumeContextScope, next).ConfigureAwait(false);
            }

            return CreateScope();
        }

        public async Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken)
            where T : class
        {
            using var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

            var factory = serviceScope.ServiceProvider.GetRequiredService<ISagaRepositoryContextFactory<TSaga>>();

            return await factory.Execute(asyncMethod, cancellationToken).ConfigureAwait(false);
        }
    }
}
