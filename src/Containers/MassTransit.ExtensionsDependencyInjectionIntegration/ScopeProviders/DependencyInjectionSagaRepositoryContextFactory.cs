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
    using Scoping.SagaContexts;


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

        public Task<SagaRepositoryContext<TSaga, T>> CreateContext<T>(ConsumeContext<T> context, Guid? correlationId)
            where T : class
        {
            if (!context.TryGetPayload(out IServiceProvider serviceProvider))
                serviceProvider = _serviceProvider;

            if (context.TryGetPayload<IServiceScope>(out var existingServiceScope))
            {
                existingServiceScope.UpdateScope(context);

                context.GetOrAddPayload(() => existingServiceScope.ServiceProvider.GetService<IStateMachineActivityFactory>()
                    ?? new DependencyInjectionStateMachineActivityFactory());

                var factory = existingServiceScope.ServiceProvider.GetRequiredService<ISagaRepositoryContextFactory<TSaga>>();

                return factory.CreateContext(context, correlationId);
            }

            async Task<SagaRepositoryContext<TSaga, T>> CreateScope()
            {
                var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
                try
                {
                    serviceScope.UpdateScope(context);

                    var scopeServiceProvider = serviceScope.ServiceProvider;

                    var factory = scopeServiceProvider.GetRequiredService<ISagaRepositoryContextFactory<TSaga>>();

                    var activityFactory = scopeServiceProvider.GetService<IStateMachineActivityFactory>()
                        ?? new DependencyInjectionStateMachineActivityFactory();

                    var consumeContextScope = new ConsumeContextScope<T>(context, serviceScope, scopeServiceProvider, activityFactory);

                    SagaRepositoryContext<TSaga, T> repositoryContext = await factory.CreateContext(consumeContextScope, correlationId).ConfigureAwait(false);

                    return new ScopeRepositoryContext<TSaga, T>(repositoryContext, serviceScope);
                }
                catch
                {
                    serviceScope.Dispose();

                    throw;
                }
            }

            return CreateScope();
        }

        public async Task<SagaRepositoryContext<TSaga>> CreateContext(CancellationToken cancellationToken = default, Guid? correlationId = default)
        {
            var factory = _serviceProvider.GetRequiredService<ISagaRepositoryContextFactory<TSaga>>();

            var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            try
            {
                SagaRepositoryContext<TSaga> repositoryContext = await factory.CreateContext(cancellationToken, correlationId).ConfigureAwait(false);

                var scopeRepositoryContext = new ScopeRepositoryContext<TSaga>(repositoryContext, serviceScope);

                return scopeRepositoryContext;
            }
            catch
            {
                serviceScope.Dispose();

                throw;
            }
        }
    }
}
