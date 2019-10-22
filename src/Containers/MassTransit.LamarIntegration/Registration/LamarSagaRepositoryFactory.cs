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
        readonly Action<ConsumeContext> _configureScope;
        readonly IContainer _container;

        public LamarSagaRepositoryFactory(IContainer container)
        {
            _container = container;
        }

        public LamarSagaRepositoryFactory(IContainer container, Action<ConsumeContext> configureScope)
        {
            _container = container;
            _configureScope = configureScope;
        }

        ISagaRepository<T> ISagaRepositoryFactory.CreateSagaRepository<T>(Action<ConsumeContext> scopeAction)
        {
            var repository = _container.GetInstance<ISagaRepository<T>>();

            var scopeProvider = new LamarSagaScopeProvider<T>(_container);
            if (scopeAction != null)
                scopeProvider.AddScopeAction(scopeAction);

            if (_configureScope != null)
                scopeProvider.AddScopeAction(_configureScope);

            return new ScopeSagaRepository<T>(repository, scopeProvider);
        }
    }
}
