namespace MassTransit.StructureMapIntegration.ScopeProviders
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Automatonymous;
    using Context;
    using GreenPipes;
    using Saga;
    using StructureMap;


    public class StructureMapSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly IContainer _container;
        readonly IContext _context;

        public StructureMapSagaRepositoryContextFactory(IContainer container)
        {
            _container = container;
        }

        public StructureMapSagaRepositoryContextFactory(IContext context)
        {
            _context = context;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "structuremap");
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
            var nestedContainer = _container?.GetNestedContainer() ?? _context?.GetInstance<IContainer>().GetNestedContainer();

            var factory = nestedContainer.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

            return await factory.Execute(asyncMethod, cancellationToken).ConfigureAwait(false);
        }

        Task Send<T>(ConsumeContext<T> context, Func<ConsumeContext<T>, ISagaRepositoryContextFactory<TSaga>, Task> send)
            where T : class
        {
            if (context.TryGetPayload<IContainer>(out var existingContainer))
            {
                existingContainer.Inject<ConsumeContext>(context);

                context.GetOrAddPayload(() => existingContainer.TryGetInstance<IStateMachineActivityFactory>()
                    ?? StructureMapStateMachineActivityFactory.Instance);

                var factory = existingContainer.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

                return send(context, factory);
            }

            async Task CreateNestedContainer()
            {
                using var nestedContainer = _container?.CreateNestedContainer(context) ?? _context?.CreateNestedContainer(context);

                var activityFactory = nestedContainer.TryGetInstance<IStateMachineActivityFactory>()
                    ?? new StructureMapStateMachineActivityFactory();

                var consumeContextScope = new ConsumeContextScope<T>(context, nestedContainer, activityFactory);

                var factory = nestedContainer.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

                await send(consumeContextScope, factory).ConfigureAwait(false);
            }

            return CreateNestedContainer();
        }
    }
}
