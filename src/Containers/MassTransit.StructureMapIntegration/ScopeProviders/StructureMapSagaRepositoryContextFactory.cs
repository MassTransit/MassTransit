namespace MassTransit.StructureMapIntegration.ScopeProviders
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Automatonymous;
    using Context;
    using GreenPipes;
    using Saga;
    using Scoping.SagaContexts;
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

        public Task<SagaRepositoryContext<TSaga, T>> CreateContext<T>(ConsumeContext<T> context, Guid? correlationId)
            where T : class
        {
            if (context.TryGetPayload<IContainer>(out var existingNestedContainer))
            {
                existingNestedContainer.Inject(context);
                existingNestedContainer.Inject<ConsumeContext>(context);

                context.GetOrAddPayload(() => existingNestedContainer.TryGetInstance<IStateMachineActivityFactory>()
                    ?? new StructureMapStateMachineActivityFactory());

                var factory = existingNestedContainer.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

                return factory.CreateContext(context, correlationId);
            }

            async Task<SagaRepositoryContext<TSaga, T>> CreateNestedContainer()
            {
                var nestedContainer = _container?.CreateNestedContainer(context) ?? _context?.CreateNestedContainer(context);
                try
                {
                    var factory = nestedContainer.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

                    var activityFactory = nestedContainer.TryGetInstance<IStateMachineActivityFactory>()
                        ?? new StructureMapStateMachineActivityFactory();

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
            var nestedContainer = _container?.GetNestedContainer() ?? _context?.GetInstance<IContainer>().GetNestedContainer();
            try
            {
                var factory = nestedContainer.GetInstance<ISagaRepositoryContextFactory<TSaga>>();

                SagaRepositoryContext<TSaga> repositoryContext = await factory.CreateContext(cancellationToken, correlationId).ConfigureAwait(false);

                var scopeRepositoryContext = new ScopeRepositoryContext<TSaga>(repositoryContext, nestedContainer);

                return scopeRepositoryContext;
            }
            catch
            {
                nestedContainer.Dispose();

                throw;
            }
        }
    }
}
