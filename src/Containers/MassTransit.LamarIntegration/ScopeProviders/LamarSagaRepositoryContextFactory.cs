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

        public Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class
        {
            if (context.TryGetPayload<INestedContainer>(out var existingContainer))
            {
                existingContainer.Inject<ConsumeContext>(context);

                context.GetOrAddPayload(() => existingContainer.TryGetInstance<IStateMachineActivityFactory>()
                    ?? LamarStateMachineActivityFactory.Instance);

                var factory = existingContainer.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

                return factory.Send(context, next);
            }

            async Task CreateNestedContainer()
            {
                using var nestedContainer = _container.GetNestedContainer(context);

                var activityFactory = nestedContainer.TryGetInstance<IStateMachineActivityFactory>()
                    ?? LamarStateMachineActivityFactory.Instance;

                var consumeContextScope = new ConsumeContextScope<T>(context, nestedContainer, activityFactory);

                var factory = nestedContainer.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

                await factory.Send(consumeContextScope, next).ConfigureAwait(false);
            }

            return CreateNestedContainer();
        }

        public async Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken)
            where T : class
        {
            using var nestedContainer = _container.GetNestedContainer();

            var factory = nestedContainer.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

            return await factory.Execute(asyncMethod, cancellationToken).ConfigureAwait(false);
        }
    }
}
