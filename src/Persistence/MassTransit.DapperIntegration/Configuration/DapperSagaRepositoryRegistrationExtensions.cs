namespace MassTransit
{
    using System;
    using Configuration;
    using DapperIntegration.Configuration;


    public static class DapperSagaRepositoryRegistrationExtensions
    {
        /// <summary>
        /// Adds a Redis saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<T> DapperRepository<T>(this ISagaRegistrationConfigurator<T> configurator,
            string connectionString, Action<IDapperSagaRepositoryConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            var repositoryConfigurator = new DapperSagaRepositoryConfigurator<T>(connectionString);

            configure?.Invoke(repositoryConfigurator);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The Dapper saga repository configuration is invalid:");

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Use the Dapper saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString"></param>
        /// <param name="configure"></param>
        public static void SetDapperSagaRepositoryProvider(this IRegistrationConfigurator configurator, string connectionString,
            Action<IDapperSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new DapperSagaRepositoryRegistrationProvider(connectionString, configure));
        }
    }
}
