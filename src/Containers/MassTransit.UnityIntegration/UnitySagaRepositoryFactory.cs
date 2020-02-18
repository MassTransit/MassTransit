namespace MassTransit.UnityIntegration
{
    using System;
    using Registration;
    using Saga;
    using Scoping;
    using Unity;


    public class UnitySagaRepositoryFactory :
        ISagaRepositoryFactory
    {
        readonly IUnityContainer _container;

        public UnitySagaRepositoryFactory(IUnityContainer container)
        {
            _container = container;
        }

        public ISagaRepository<T> CreateSagaRepository<T>(Action<ConsumeContext> scopeAction)
            where T : class, ISaga
        {
            var repository = _container.Resolve<ISagaRepository<T>>();
            if (repository is SagaRepository<T>)
                return repository;

            var scopeProvider = new UnitySagaScopeProvider<T>(_container);

            return new ScopeSagaRepository<T>(repository, scopeProvider);
        }
    }
}
