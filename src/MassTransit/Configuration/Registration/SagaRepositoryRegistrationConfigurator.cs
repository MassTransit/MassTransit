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

        void ISagaRepositoryRegistrationConfigurator<TSaga>.RegisterFactoryMethod(Func<IConfigurationServiceProvider, ISagaRepository<TSaga>> repositoryFactory)
        {
            _registrar.RegisterSagaRepository(repositoryFactory);
        }

        void ISagaRepositoryRegistrationConfigurator<TSaga>.RegisterComponents<TContext, TConsumeContextFactory, TRepositoryContextFactory>()
        {
            _registrar.RegisterSagaRepository<TSaga, TContext, TConsumeContextFactory, TRepositoryContextFactory>();
        }

        void ISagaRepositoryRegistrationConfigurator<TSaga>.RegisterInstance<T>(Func<IConfigurationServiceProvider, T> factoryMethod)
        {
            _registrar.RegisterInstance(factoryMethod);
        }

        void ISagaRepositoryRegistrationConfigurator<TSaga>.RegisterInstance<T>(T instance)
        {
            _registrar.RegisterInstance(instance);
        }

        public void RegisterScoped<T, TImplementation>()
            where T : class
            where TImplementation : class, T
        {
        }
    }
}
