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

        void RegisterComponents<TContext, TConsumeContextFactory, TRepositoryContextFactory>()
            where TContext : class
            where TConsumeContextFactory : class, ISagaConsumeContextFactory<TContext, TSaga>
            where TRepositoryContextFactory : class, ISagaRepositoryContextFactory<TSaga>;

        void RegisterInstance<T>(Func<IConfigurationServiceProvider, T> factoryMethod)
            where T : class;

        void RegisterInstance<T>(T instance)
            where T : class;

        void RegisterScoped<T, TImplementation>()
            where T : class
            where TImplementation : class, T;
    }
}
