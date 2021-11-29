namespace MassTransit
{
    using System;
    using Configuration;
    using Marten;
    using Npgsql;


    public static class MartenSagaRepositoryRegistrationExtensions
    {
        /// <summary>
        /// Adds a Marten saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString">The Marten configuration string</param>
        /// <param name="configureOptions"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<T> MartenRepository<T>(this ISagaRegistrationConfigurator<T> configurator, string connectionString,
            Action<StoreOptions> configureOptions)
            where T : class, ISaga
        {
            var repositoryConfigurator = new MartenSagaRepositoryConfigurator<T>();

            repositoryConfigurator.Connection(connectionString, configureOptions);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The Marten saga repository configuration is invalid:");

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Adds a Marten saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionFactory"></param>
        /// <param name="configureOptions"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<T> MartenRepository<T>(this ISagaRegistrationConfigurator<T> configurator,
            Func<NpgsqlConnection> connectionFactory, Action<StoreOptions> configureOptions)
            where T : class, ISaga
        {
            var repositoryConfigurator = new MartenSagaRepositoryConfigurator<T>();

            repositoryConfigurator.Connection(connectionFactory, configureOptions);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The Marten saga repository configuration is invalid:");

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Adds a Marten saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString">The Marten configuration string</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<T> MartenRepository<T>(this ISagaRegistrationConfigurator<T> configurator, string connectionString)
            where T : class, ISaga
        {
            var repositoryConfigurator = new MartenSagaRepositoryConfigurator<T>();

            repositoryConfigurator.Connection(connectionString);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The Marten saga repository configuration is invalid:");

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Use the Marten saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString"></param>
        public static void SetMartenSagaRepositoryProvider(this IRegistrationConfigurator configurator, string connectionString)
        {
            configurator.SetSagaRepositoryProvider(new MartenSagaRepositoryRegistrationProvider(connectionString, x =>
            {
            }));
        }

        /// <summary>
        /// Use the Marten saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString"></param>
        /// <param name="configureOptions"></param>
        public static void SetMartenSagaRepositoryProvider(this IRegistrationConfigurator configurator, string connectionString,
            Action<StoreOptions> configureOptions)
        {
            configurator.SetSagaRepositoryProvider(new MartenSagaRepositoryRegistrationProvider(connectionString, configureOptions));
        }

        /// <summary>
        /// Use the Marten saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionFactory"></param>
        /// <param name="configureOptions"></param>
        public static void SetMartenSagaRepositoryProvider(this IRegistrationConfigurator configurator, Func<NpgsqlConnection> connectionFactory,
            Action<StoreOptions> configureOptions)
        {
            configurator.SetSagaRepositoryProvider(new MartenSagaRepositoryRegistrationProvider(connectionFactory, configureOptions));
        }
    }
}
