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
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<T> MartenRepository<T>(this ISagaRegistrationConfigurator<T> configurator)
            where T : class, ISaga
        {
            var repositoryConfigurator = new MartenSagaRepositoryConfigurator<T>();

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Adds a Marten saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString">The Marten configuration string</param>
        /// <param name="configureOptions"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete("AddMarten should be used to set up and configure Marten and use MartenRepository() with no arguments.")]
        public static ISagaRegistrationConfigurator<T> MartenRepository<T>(this ISagaRegistrationConfigurator<T> configurator, string connectionString,
            Action<StoreOptions> configureOptions)
            where T : class, ISaga
        {
            var repositoryConfigurator = new MartenSagaRepositoryConfigurator<T>();

            repositoryConfigurator.Connection(connectionString, configureOptions);

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
        [Obsolete("AddMarten should be used to set up and configure Marten and use MartenRepository() with no arguments.")]
        public static ISagaRegistrationConfigurator<T> MartenRepository<T>(this ISagaRegistrationConfigurator<T> configurator,
            Func<NpgsqlConnection> connectionFactory, Action<StoreOptions> configureOptions)
            where T : class, ISaga
        {
            var repositoryConfigurator = new MartenSagaRepositoryConfigurator<T>();

            repositoryConfigurator.Connection(connectionFactory, configureOptions);

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
        [Obsolete("AddMarten should be used to set up and configure Marten and use MartenRepository() with no arguments.")]
        public static ISagaRegistrationConfigurator<T> MartenRepository<T>(this ISagaRegistrationConfigurator<T> configurator, string connectionString)
            where T : class, ISaga
        {
            var repositoryConfigurator = new MartenSagaRepositoryConfigurator<T>();

            repositoryConfigurator.Connection(connectionString);

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Use the Marten saga repository for sagas configured by type (without a specific repository specified via AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        public static void SetMartenSagaRepositoryProvider(this IRegistrationConfigurator configurator)
        {
            configurator.SetSagaRepositoryProvider(new MartenSagaRepositoryRegistrationProvider());
        }

        /// <summary>
        /// Use the Marten saga repository for sagas configured by type (without a specific repository specified via AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString"></param>
        [Obsolete("AddMarten should be used to set up and configure Marten and use SetMartenSagaRepositoryProvider() with no arguments.")]
        public static void SetMartenSagaRepositoryProvider(this IRegistrationConfigurator configurator, string connectionString)
        {
            configurator.AddMarten(options =>
            {
                options.Connection(connectionString);
            });

            configurator.SetSagaRepositoryProvider(new MartenSagaRepositoryRegistrationProvider());
        }

        /// <summary>
        /// Use the Marten saga repository for sagas configured by type (without a specific repository specified via AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString"></param>
        /// <param name="configureOptions"></param>
        [Obsolete("AddMarten should be used to set up and configure Marten and use SetMartenSagaRepositoryProvider() with no arguments.")]
        public static void SetMartenSagaRepositoryProvider(this IRegistrationConfigurator configurator, string connectionString,
            Action<StoreOptions> configureOptions)
        {
            configurator.AddMarten(options =>
            {
                options.Connection(connectionString);

                configureOptions?.Invoke(options);
            });

            configurator.SetSagaRepositoryProvider(new MartenSagaRepositoryRegistrationProvider());
        }

        /// <summary>
        /// Use the Marten saga repository for sagas configured by type (without a specific repository specified via AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionFactory"></param>
        /// <param name="configureOptions"></param>
        [Obsolete("AddMarten should be used to set up and configure Marten and use SetMartenSagaRepositoryProvider() with no arguments.")]
        public static void SetMartenSagaRepositoryProvider(this IRegistrationConfigurator configurator, Func<NpgsqlConnection> connectionFactory,
            Action<StoreOptions> configureOptions)
        {
            configurator.AddMarten(options =>
            {
                options.Connection(connectionFactory);

                configureOptions?.Invoke(options);
            });

            configurator.SetSagaRepositoryProvider(new MartenSagaRepositoryRegistrationProvider());
        }
    }
}
