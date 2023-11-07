namespace MassTransit.Configuration
{
    using System;
    using Configuration;


    public static class CassandraDbSagaRepositoryRegistrationExtensions
    {
        /// <summary>
        /// Adds a CassandraDb saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<T> CassandraDbRepository<T>(this ISagaRegistrationConfigurator<T> configurator,
            Action<ICassandraDbSagaRepositoryConfigurator<T>> configure = null)
            where T : class, ISagaVersion
        {
            var repositoryConfigurator = new CassandraDbSagaRepositoryConfigurator<T>();

            configure?.Invoke(repositoryConfigurator);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The CassandraDb saga repository configuration is invalid:");

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Use the CassandraDb saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void SetCassandraDbSagaRepositoryProvider(this IRegistrationConfigurator configurator,
            Action<ICassandraDbSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new CassandraDbSagaRepositoryRegistrationProvider(configure));
        }
    }
}
