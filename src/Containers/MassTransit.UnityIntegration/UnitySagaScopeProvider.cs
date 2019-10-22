namespace MassTransit.UnityIntegration
{
    using Context;
    using GreenPipes;
    using Saga;
    using Scoping;
    using Scoping.SagaContexts;
    using Unity;


    public class UnitySagaScopeProvider<TSaga> :
        ISagaScopeProvider<TSaga>
        where TSaga : class, ISaga
    {
        readonly IUnityContainer _container;

        public UnitySagaScopeProvider(IUnityContainer container)
        {
            _container = container;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("provider", "unity");
        }

        ISagaScopeContext<T> ISagaScopeProvider<TSaga>.GetScope<T>(ConsumeContext<T> context)
        {
            if (context.TryGetPayload<IUnityContainer>(out var existingScope))
                return new ExistingSagaScopeContext<T>(context);

            var scope = _container.CreateChildContainer();
            try
            {
                var proxy = new ConsumeContextScope<T>(context, scope);

                return new CreatedSagaScopeContext<IUnityContainer, T>(scope, proxy);
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }

        ISagaQueryScopeContext<TSaga, T> ISagaScopeProvider<TSaga>.GetQueryScope<T>(SagaQueryConsumeContext<TSaga, T> context)
        {
            if (context.TryGetPayload<IUnityContainer>(out var existingScope))
                return new ExistingSagaQueryScopeContext<TSaga, T>(context);

            var scope = _container.CreateChildContainer();
            try
            {
                var proxy = new SagaQueryConsumeContextScope<TSaga, T>(context, context.Query, scope);

                return new CreatedSagaQueryScopeContext<IUnityContainer, TSaga, T>(scope, proxy);
            }
            catch
            {
                scope.Dispose();

                throw;
            }
        }
    }
}
