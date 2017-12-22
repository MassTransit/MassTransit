namespace MassTransit.AutomatonymousExtensionsDependencyInjectionIntegration
{
    using System;
    using Automatonymous;
    using ExtensionsDependencyInjectionIntegration;
    using Saga;
    using Scoping;
    using Microsoft.Extensions.DependencyInjection;



    public class DependencyInjectionStateMachineSagaRepositoryFactory :
        ISagaRepositoryFactory
    {
        readonly IServiceProvider _scopeProvider;

        public DependencyInjectionStateMachineSagaRepositoryFactory(IServiceProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        ISagaRepository<T> ISagaRepositoryFactory.CreateSagaRepository<T>()
        {
            var repository = _scopeProvider.GetRequiredService<ISagaRepository<T>>();

            var scopeProvider = new DependencyInjectionSagaScopeProvider<T>(_scopeProvider);

            scopeProvider.AddScopeAction(x => x.GetOrAddPayload<IStateMachineActivityFactory>(() => new DependencyInjectionStateMachineActivityFactory()));

            return new ScopeSagaRepository<T>(repository, scopeProvider);
        }
    }
}