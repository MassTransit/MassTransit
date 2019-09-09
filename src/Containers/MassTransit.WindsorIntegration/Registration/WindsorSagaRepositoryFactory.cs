namespace MassTransit.WindsorIntegration.Registration
{
    using System;
    using Castle.MicroKernel;
    using MassTransit.Registration;
    using Saga;
    using ScopeProviders;
    using Scoping;


    public class WindsorSagaRepositoryFactory :
        ISagaRepositoryFactory
    {
        readonly IKernel _kernel;

        public WindsorSagaRepositoryFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public ISagaRepository<T> CreateSagaRepository<T>(Action<ConsumeContext> scopeAction)
            where T : class, ISaga
        {
            var repository = _kernel.Resolve<ISagaRepository<T>>();

            var scopeProvider = new WindsorSagaScopeProvider<T>(_kernel);
            if (scopeAction != null)
                scopeProvider.AddScopeAction(scopeAction);

            return new ScopeSagaRepository<T>(repository, scopeProvider);
        }
    }
}
