namespace MassTransit.AutomatonymousAutofacIntegration
{
    using System;
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
        readonly Action<ContainerBuilder, ConsumeContext> _configureScope;

        public AutofacStateMachineSagaRepositoryFactory(ILifetimeScopeProvider scopeProvider, string name, Action<ContainerBuilder, ConsumeContext> configureScope)
        {
            _scopeProvider = scopeProvider;
            _name = name;
            _configureScope = configureScope;
        }

        ISagaRepository<T> ISagaRepositoryFactory.CreateSagaRepository<T>()
        {
            var repository = _scopeProvider.LifetimeScope.Resolve<ISagaRepository<T>>();

            var scopeProvider = new AutofacSagaScopeProvider<T>(_scopeProvider, _name, _configureScope);

            scopeProvider.AddScopeAction(x => x.GetOrAddPayload<IStateMachineActivityFactory>(() => new AutofacStateMachineActivityFactory()));

            return new ScopeSagaRepository<T>(repository, scopeProvider);
        }
    }
}