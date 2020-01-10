namespace MassTransit.SimpleInjectorIntegration.Registration
{
    using System;
    using MassTransit.Registration;
    using Saga;
    using ScopeProviders;
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

        public ISagaRepository<T> CreateSagaRepository<T>(Action<ConsumeContext> scopeAction)
            where T : class, ISaga
        {
            var repository = _container.GetInstance<ISagaRepository<T>>();
            if (repository is SagaRepository<T>)
                return repository;

            var scopeProvider = new SimpleInjectorSagaScopeProvider<T>(_container);
            if (scopeAction != null)
                scopeProvider.AddScopeAction(scopeAction);

            return new ScopeSagaRepository<T>(repository, scopeProvider);
        }
    }
}
