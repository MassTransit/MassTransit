namespace MassTransit.AutofacIntegration
{
    using Autofac;
    using Saga;
    using Scoping;


    public class AutofacSagaRepositoryFactory :
        ISagaRepositoryFactory
    {
        readonly ILifetimeScopeProvider _scopeProvider;
        readonly string _name;

        public AutofacSagaRepositoryFactory(ILifetimeScopeProvider scopeProvider, string name)
        {
            _scopeProvider = scopeProvider;
            _name = name;
        }

        ISagaRepository<T> ISagaRepositoryFactory.CreateSagaRepository<T>()
        {
            ISagaRepository<T> repository = _scopeProvider.LifetimeScope.Resolve<ISagaRepository<T>>();
            
            var scopeProvider = new AutofacSagaScopeProvider<T>(_scopeProvider, _name);

            return new ScopeSagaRepository<T>(repository, scopeProvider);
        }
    }
}