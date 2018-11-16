namespace MassTransit.AutomatonymousWindsorIntegration
{
    using Automatonymous;
    using Castle.MicroKernel;
    using Castle.Windsor;
    using Saga;
    using Scoping;

    public class WindsorStateMachineSagaRepositoryFactory : ISagaRepositoryFactory
    {
        readonly IKernel _container;

        public WindsorStateMachineSagaRepositoryFactory(IKernel container)
        {
            _container = container;
        }

        public ISagaRepository<T> CreateSagaRepository<T>()
            where T : class, ISaga
        {
            var repository = _container.Resolve<ISagaRepository<T>>();

            var scopeProvider = new WindsorSagaScopeProvider<T>(_container);

            scopeProvider.AddScopeAction(x => x.GetOrAddPayload<IStateMachineActivityFactory>(() => new WindsorStateMachineActivityFactory()));

            return new ScopeSagaRepository<T>(repository, scopeProvider);
        }
    }
}
