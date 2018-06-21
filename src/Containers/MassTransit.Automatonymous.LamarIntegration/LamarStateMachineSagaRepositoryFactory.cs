namespace MassTransit.AutomatonymousLamarIntegration
{
    using Automatonymous;
    using Lamar;
    using LamarIntegration;
    using Saga;
    using Scoping;


    public class LamarStateMachineSagaRepositoryFactory :
        ISagaRepositoryFactory
    {
        readonly IContainer _container;

        public LamarStateMachineSagaRepositoryFactory(IContainer container)
        {
            _container = container;
        }

        ISagaRepository<T> ISagaRepositoryFactory.CreateSagaRepository<T>()
        {
            var repository = _container.GetInstance<ISagaRepository<T>>();

            var scopeProvider = new LamarSagaScopeProvider<T>(_container);

            scopeProvider.AddScopeAction(x => x.GetOrAddPayload<IStateMachineActivityFactory>(() => new LamarStateMachineActivityFactory()));

            return new ScopeSagaRepository<T>(repository, scopeProvider);
        }
    }
}
