namespace MassTransit
{
    using System;
    using Registration;
    using Saga;


    public interface ISagaRepositoryRegistrationConfigurator<TSaga> :
        IContainerRegistrar
        where TSaga : class, ISaga
    {
        /// <summary>
        /// Register a factory method in the container to create the saga repository.
        /// </summary>
        /// <param name="repositoryFactory"></param>
        void RegisterFactoryMethod(Func<IConfigurationServiceProvider, ISagaRepository<TSaga>> repositoryFactory);

        void RegisterComponents<TContext, TConsumeContextFactory, TRepositoryContextFactory>()
            where TContext : class
            where TConsumeContextFactory : class, ISagaConsumeContextFactory<TContext, TSaga>
            where TRepositoryContextFactory : class, ISagaRepositoryContextFactory<TSaga>;
    }
}
