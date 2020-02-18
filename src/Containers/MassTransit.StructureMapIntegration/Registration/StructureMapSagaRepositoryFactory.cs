namespace MassTransit.StructureMapIntegration.Registration
{
    using System;
    using MassTransit.Registration;
    using Saga;
    using ScopeProviders;
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

        ISagaRepository<T> ISagaRepositoryFactory.CreateSagaRepository<T>(Action<ConsumeContext> scopeAction)
        {
            var repository = _container.GetInstance<ISagaRepository<T>>();
            if (repository is SagaRepository<T>)
                return repository;

            var scopeProvider = new StructureMapSagaScopeProvider<T>(_container);
            if (scopeAction != null)
                scopeProvider.AddScopeAction(scopeAction);

            return new ScopeSagaRepository<T>(repository, scopeProvider);
        }
    }
}
