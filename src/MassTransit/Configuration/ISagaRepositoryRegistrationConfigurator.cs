namespace MassTransit
{
    using System;
    using Registration;
    using Saga;


    public interface ISagaRepositoryRegistrationConfigurator<TSaga>
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Register a factory method in the container to create the saga repository.
        /// </summary>
        /// <param name="repositoryFactory"></param>
        void RegisterFactoryMethod(Func<IConfigurationServiceProvider, ISagaRepository<TSaga>> repositoryFactory);
    }
}
