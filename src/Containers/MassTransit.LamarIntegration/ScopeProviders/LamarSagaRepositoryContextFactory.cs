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
            using var nestedContainer = _container.GetNestedContainer();

            var factory = nestedContainer.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

            return await factory.Execute(asyncMethod, cancellationToken).ConfigureAwait(false);
        }

        Task Send<T>(ConsumeContext<T> context, Func<ConsumeContext<T>, ISagaRepositoryContextFactory<TSaga>, Task> send)
            where T : class
        {
            if (context.TryGetPayload<INestedContainer>(out var existingContainer))
            {
                existingContainer.Inject<ConsumeContext>(context);

                context.GetOrAddPayload(() => existingContainer.TryGetInstance<IStateMachineActivityFactory>()
                    ?? LamarStateMachineActivityFactory.Instance);

                var factory = existingContainer.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

                return send(context, factory);
            }

            async Task CreateNestedContainer()
            {
                using var nestedContainer = _container.GetNestedContainer(context);

                var activityFactory = nestedContainer.TryGetInstance<IStateMachineActivityFactory>()
                    ?? LamarStateMachineActivityFactory.Instance;

                var consumeContextScope = new ConsumeContextScope<T>(context, nestedContainer, activityFactory);

                var factory = nestedContainer.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

                await send(consumeContextScope, factory).ConfigureAwait(false);
            }

            return CreateNestedContainer();
        }
    }
}
