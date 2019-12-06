namespace MassTransit.Registration
{
    using System;
    using Saga;


    public class SagaRepositoryRegistrationConfigurator<TSaga> :
        ISagaRepositoryRegistrationConfigurator<TSaga>
        where TSaga : class, ISaga
    {
        readonly IContainerRegistrar _registrar;

        public SagaRepositoryRegistrationConfigurator(IContainerRegistrar registrar)
        {
            _registrar = registrar;
        }

        public void RegisterFactoryMethod(Func<IConfigurationServiceProvider, ISagaRepository<TSaga>> repositoryFactory)
        {
            _registrar.RegisterSagaRepository(repositoryFactory);
        }
    }
}
