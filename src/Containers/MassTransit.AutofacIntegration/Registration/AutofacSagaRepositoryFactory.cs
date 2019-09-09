namespace MassTransit.AutofacIntegration.Registration
{
    using System;
    using Autofac;
    using MassTransit.Registration;
    using Saga;
    using ScopeProviders;
    using Scoping;


    public class AutofacSagaRepositoryFactory :
        ISagaRepositoryFactory
    {
        readonly Action<ContainerBuilder, ConsumeContext> _configureScope;
        readonly string _name;
        readonly ILifetimeScopeProvider _scopeProvider;

        public AutofacSagaRepositoryFactory(ILifetimeScopeProvider scopeProvider, string name, Action<ContainerBuilder, ConsumeContext> configureScope)
        {
            _scopeProvider = scopeProvider;
            _name = name;
            _configureScope = configureScope;
        }

        ISagaRepository<T> ISagaRepositoryFactory.CreateSagaRepository<T>(Action<ConsumeContext> scopeAction)
        {
            var repository = _scopeProvider.LifetimeScope.Resolve<ISagaRepository<T>>();

            var scopeProvider = new AutofacSagaScopeProvider<T>(_scopeProvider, _name, _configureScope);
            if (scopeAction != null)
                scopeProvider.AddScopeAction(scopeAction);

            return new ScopeSagaRepository<T>(repository, scopeProvider);
        }
    }
}
