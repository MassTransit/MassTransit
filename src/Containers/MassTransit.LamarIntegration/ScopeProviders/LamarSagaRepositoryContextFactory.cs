namespace MassTransit.LamarIntegration.ScopeProviders
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Automatonymous;
    using Context;
    using GreenPipes;
    using Lamar;
    using Saga;
    using Scoping.SagaContexts;


    public class LamarSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly IContainer _container;

        public LamarSagaRepositoryContextFactory(IContainer container)
        {
            _container = container;
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            context.Add("provider", "lamar");
        }

        public Task<SagaRepositoryContext<TSaga, T>> CreateContext<T>(ConsumeContext<T> context, Guid? correlationId)
            where T : class
        {
            if (context.TryGetPayload<INestedContainer>(out var existingNestedContainer))
            {
                existingNestedContainer.Inject(context);
                existingNestedContainer.Inject<ConsumeContext>(context);

                context.GetOrAddPayload(() => existingNestedContainer.TryGetInstance<IStateMachineActivityFactory>()
                    ?? new LamarStateMachineActivityFactory());

                var factory = existingNestedContainer.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

                return factory.CreateContext(context, correlationId);
            }

            async Task<SagaRepositoryContext<TSaga, T>> CreateNestedContainer()
            {
                var nestedContainer = _container.GetNestedContainer(context);
                try
                {
                    var factory = nestedContainer.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

                    var activityFactory = nestedContainer.TryGetInstance<IStateMachineActivityFactory>()
                        ?? new LamarStateMachineActivityFactory();

                    var consumeContextScope = new ConsumeContextScope<T>(context, nestedContainer, activityFactory);

                    SagaRepositoryContext<TSaga, T> repositoryContext = await factory.CreateContext(consumeContextScope, correlationId).ConfigureAwait(false);

                    return new ScopeRepositoryContext<TSaga, T>(repositoryContext, nestedContainer);
                }
                catch
                {
                    nestedContainer.Dispose();

                    throw;
                }
            }

            return CreateNestedContainer();
        }

        public async Task<SagaRepositoryContext<TSaga>> CreateContext(CancellationToken cancellationToken = default, Guid? correlationId = default)
        {
            var nestedContainer = _container.GetNestedContainer();
            try
            {
                var factory = nestedContainer.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

                SagaRepositoryContext<TSaga> repositoryContext = await factory.CreateContext(cancellationToken, correlationId).ConfigureAwait(false);

                return new ScopeRepositoryContext<TSaga>(repositoryContext, nestedContainer);
            }
            catch
            {
                nestedContainer.Dispose();

                throw;
            }
        }
    }
}
