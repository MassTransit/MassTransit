namespace MassTransit.AutomatonymousAutofacIntegration
{
    using Autofac;
    using AutofacIntegration;
    using Automatonymous;
    using Saga;
    using Scoping;


    public class AutofacStateMachineSagaRepositoryFactory :
        ISagaRepositoryFactory
    {
        readonly ILifetimeScopeProvider _scopeProvider;
        readonly string _name;

        public AutofacStateMachineSagaRepositoryFactory(ILifetimeScopeProvider scopeProvider, string name)
        {
            _scopeProvider = scopeProvider;
            _name = name;
        }

        ISagaRepository<T> ISagaRepositoryFactory.CreateSagaRepository<T>()
        {
            var repository = _scopeProvider.LifetimeScope.Resolve<ISagaRepository<T>>();

            var scopeProvider = new AutofacSagaScopeProvider<T>(_scopeProvider, _name);

            scopeProvider.AddScopeAction(x => x.GetOrAddPayload<IStateMachineActivityFactory>(() => new AutofacStateMachineActivityFactory()));

            return new ScopeSagaRepository<T>(repository, scopeProvider);
        }
    }
}