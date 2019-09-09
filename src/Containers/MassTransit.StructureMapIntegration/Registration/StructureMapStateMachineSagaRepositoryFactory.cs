namespace MassTransit.StructureMapIntegration.Registration
{
    using System;
    using Automatonymous;
    using MassTransit.Registration;
    using Saga;
    using ScopeProviders;
    using Scoping;
    using StructureMap;


    public class StructureMapStateMachineSagaRepositoryFactory :
        ISagaRepositoryFactory
    {
        readonly IContainer _container;

        public StructureMapStateMachineSagaRepositoryFactory(IContainer container)
        {
            _container = container;
        }

        ISagaRepository<T> ISagaRepositoryFactory.CreateSagaRepository<T>(Action<ConsumeContext> scopeAction)
        {
            var repository = _container.GetInstance<ISagaRepository<T>>();

            var scopeProvider = new StructureMapSagaScopeProvider<T>(_container);

            scopeProvider.AddScopeAction(x => x.GetOrAddPayload<IStateMachineActivityFactory>(() => new StructureMapStateMachineActivityFactory()));

            return new ScopeSagaRepository<T>(repository, scopeProvider);
        }
    }
}
