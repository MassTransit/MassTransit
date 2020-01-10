namespace MassTransit.SimpleInjectorIntegration.ScopeProviders
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Automatonymous;
    using Context;
    using GreenPipes;
    using Saga;
    using Scoping.SagaContexts;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;


    public class SimpleInjectorSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly Container _container;

        public SimpleInjectorSagaRepositoryContextFactory(Container container)
        {
            _container = container;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("provider", "simpleInjector");
        }

        public Task<SagaRepositoryContext<TSaga, T>> CreateContext<T>(ConsumeContext<T> context, Guid? correlationId)
            where T : class
        {
            if (context.TryGetPayload<Scope>(out var existingScope))
            {
                existingScope.UpdateScope(context);

                context.GetOrAddPayload(() => existingScope.TryGetInstance<IStateMachineActivityFactory>()
                    ?? new SimpleInjectorStateMachineActivityFactory());

                var factory = existingScope.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

                return factory.CreateContext(context, correlationId);
            }

            async Task<SagaRepositoryContext<TSaga, T>> CreateNestedContainer()
            {
                var scope = AsyncScopedLifestyle.BeginScope(_container);
                try
                {
                    scope.UpdateScope(context);

                    var factory = scope.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

                    var activityFactory = scope.TryGetInstance<IStateMachineActivityFactory>()
                        ?? new SimpleInjectorStateMachineActivityFactory();

                    var consumeContextScope = new ConsumeContextScope<T>(context, scope, activityFactory);

                    SagaRepositoryContext<TSaga, T> repositoryContext = await factory.CreateContext(consumeContextScope, correlationId).ConfigureAwait(false);

                    return new ScopeRepositoryContext<TSaga, T>(repositoryContext, scope);
                }
                catch
                {
                    scope.Dispose();

                    throw;
                }
            }

            return CreateNestedContainer();
        }

        public async Task<SagaRepositoryContext<TSaga>> CreateContext(CancellationToken cancellationToken = default, Guid? correlationId = default)
        {
            var scope = AsyncScopedLifestyle.BeginScope(_container);
            try
            {
                var factory = scope.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

                SagaRepositoryContext<TSaga> repositoryContext = await factory.CreateContext(cancellationToken, correlationId).ConfigureAwait(false);

                return new ScopeRepositoryContext<TSaga>(repositoryContext, scope);
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }
    }
}
