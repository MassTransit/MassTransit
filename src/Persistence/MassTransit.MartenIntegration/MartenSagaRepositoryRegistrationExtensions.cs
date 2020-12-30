namespace MassTransit
{
    using System;
    using Configurators;
    using Marten;
    using MartenIntegration.Configurators;
    using Npgsql;
    using Saga;


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
            var martenConfigurator = new MartenSagaRepositoryConfigurator<T>();

            martenConfigurator.Connection(connectionString, configureOptions);

            BusConfigurationResult.CompileResults(martenConfigurator.Validate());

            configurator.Repository(x => martenConfigurator.Register(x));

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
            var martenConfigurator = new MartenSagaRepositoryConfigurator<T>();

            martenConfigurator.Connection(connectionFactory, configureOptions);

            BusConfigurationResult.CompileResults(martenConfigurator.Validate());

            configurator.Repository(x => martenConfigurator.Register(x));

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
            var martenConfigurator = new MartenSagaRepositoryConfigurator<T>();

            martenConfigurator.Connection(connectionString);

            BusConfigurationResult.CompileResults(martenConfigurator.Validate());

            configurator.Repository(x => martenConfigurator.Register(x));

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
