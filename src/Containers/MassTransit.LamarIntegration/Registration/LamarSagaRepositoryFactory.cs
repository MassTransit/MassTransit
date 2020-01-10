namespace MassTransit.LamarIntegration.Registration
{
    using System;
    using Lamar;
    using MassTransit.Registration;
    using Saga;
    using ScopeProviders;
    using Scoping;


    public class LamarSagaRepositoryFactory :
        ISagaRepositoryFactory
    {
        readonly IContainer _container;

        public LamarSagaRepositoryFactory(IContainer container)
        {
            _container = container;
        }

        ISagaRepository<T> ISagaRepositoryFactory.CreateSagaRepository<T>(Action<ConsumeContext> scopeAction)
        {
            var repository = _container.GetInstance<ISagaRepository<T>>();
            if (repository is SagaRepository<T>)
                return repository;

            var scopeProvider = new LamarSagaScopeProvider<T>(_container);
            if (scopeAction != null)
                scopeProvider.AddScopeAction(scopeAction);

            return new ScopeSagaRepository<T>(repository, scopeProvider);
        }
    }
}
