namespace MassTransit.ExtensionsDependencyInjectionIntegration.Registration
{
    using System;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Saga;
    using ScopeProviders;
    using Scoping;


    public class DependencyInjectionSagaRepositoryFactory :
        ISagaRepositoryFactory
    {
        readonly IServiceProvider _serviceProvider;

        public DependencyInjectionSagaRepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        ISagaRepository<T> ISagaRepositoryFactory.CreateSagaRepository<T>(Action<ConsumeContext> scopeAction)
        {
            var repository = _serviceProvider.GetRequiredService<ISagaRepository<T>>();

            var scopeProvider = new DependencyInjectionSagaScopeProvider<T>(_serviceProvider);
            if (scopeAction != null)
                scopeProvider.AddScopeAction(scopeAction);

            return new ScopeSagaRepository<T>(repository, scopeProvider);
        }
    }
}
