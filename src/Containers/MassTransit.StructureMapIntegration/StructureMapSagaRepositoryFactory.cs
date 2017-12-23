namespace MassTransit.StructureMapIntegration
{
    using Saga;
    using Scoping;
    using StructureMap;


    public class StructureMapSagaRepositoryFactory :
        ISagaRepositoryFactory
    {
        readonly IContainer _container;

        public StructureMapSagaRepositoryFactory(IContainer container)
        {
            _container = container;
        }

        ISagaRepository<T> ISagaRepositoryFactory.CreateSagaRepository<T>()
        {
            var repository = _container.GetInstance<ISagaRepository<T>>();

            var scopeProvider = new StructureMapSagaScopeProvider<T>(_container);

            return new ScopeSagaRepository<T>(repository, scopeProvider);
        }
    }
}