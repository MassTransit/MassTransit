namespace MassTransit.LamarIntegration
{
    using Lamar;
    using Saga;
    using Scoping;


    public class LamarSagaRepositoryFactory : ISagaRepositoryFactory
    {
        readonly IContainer _container;

        public LamarSagaRepositoryFactory(IContainer container)
        {
            _container = container;
        }

        ISagaRepository<T> ISagaRepositoryFactory.CreateSagaRepository<T>()
        {
            var repository = _container.GetInstance<ISagaRepository<T>>();
            var scopeProvider = new LamarSagaScopeProvider<T>(_container);

            return new ScopeSagaRepository<T>(repository, scopeProvider);
        }
    }
}