namespace MassTransit
{
    using System;
    using Configuration;


    public static class EntityFrameworkSagaRepositoryRegistrationExtensions
    {
        /// <summary>
        /// Adds a EntityFramework saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="TSaga"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<TSaga> EntityFrameworkRepository<TSaga>(this ISagaRegistrationConfigurator<TSaga> configurator,
            Action<IEntityFrameworkSagaRepositoryConfigurator<TSaga>> configure)
            where TSaga : class, ISaga
        {
            var repositoryConfigurator = new EntityFrameworkSagaRepositoryConfigurator<TSaga>();

            configure?.Invoke(repositoryConfigurator);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The Entity Framework saga repository configuration is invalid:");

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Use the EntityFramework saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void SetEntityFrameworkSagaRepositoryProvider(this IRegistrationConfigurator configurator,
            Action<IEntityFrameworkSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new EntityFrameworkSagaRepositoryRegistrationProvider(configure));
        }
    }
}
