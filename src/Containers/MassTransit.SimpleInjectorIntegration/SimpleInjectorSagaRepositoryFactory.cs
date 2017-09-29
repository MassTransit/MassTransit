namespace MassTransit.SimpleInjectorIntegration
{
    using Saga;
    using Scoping;
    using SimpleInjector;


    public class SimpleInjectorSagaRepositoryFactory :
        ISagaRepositoryFactory
    {
        readonly Container _container;

        public SimpleInjectorSagaRepositoryFactory(Container container)
        {
            _container = container;
        }

        public ISagaRepository<T> CreateSagaRepository<T>() where T : class, ISaga
        {
            var repository = _container.GetInstance<ISagaRepository<T>>();

            var scopeProvider = new SimpleInjectorSagaScopeProvider<T>(_container);

            var sagaRepository = new ScopeSagaRepository<T>(repository, scopeProvider);

            return sagaRepository;
        }
    }
}