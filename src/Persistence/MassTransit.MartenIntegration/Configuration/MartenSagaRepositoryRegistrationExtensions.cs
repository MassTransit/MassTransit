namespace MassTransit
{
    using System;
    using Configuration;
    using Marten;
    using Npgsql;


    public static class MartenSagaRepositoryRegistrationExtensions
    {
        /// <summary>
        /// Configures the <typeparamref name="T" /> saga to use the Marten saga repository.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure">Optional, allows additional configuration on the saga schema</param>
        /// <typeparam name="T">The saga type</typeparam>
        public static ISagaRegistrationConfigurator<T> MartenRepository<T>(this ISagaRegistrationConfigurator<T> configurator,
            Action<MartenRegistry.DocumentMappingExpression<T>> configure = null)
            where T : class, ISaga
        {
            var repositoryConfigurator = new MartenSagaRepositoryConfigurator<T>(configure);

            configurator.Repository(x => repositoryConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Use the Marten saga repository for any sagas without an explicitly configured saga repository.
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="optimisticConcurrency">If true, optimistic concurrency will be used for any configured saga types</param>
        public static void SetMartenSagaRepositoryProvider(this IRegistrationConfigurator configurator, bool optimisticConcurrency = false)
        {
            configurator.SetSagaRepositoryProvider(new MartenSagaRepositoryRegistrationProvider(optimisticConcurrency));
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
        /// <param name="connectionString"></param>
        [Obsolete("AddMarten should be used to set up and configure Marten and use SetMartenSagaRepositoryProvider() with no arguments.")]
        public static void SetMartenSagaRepositoryProvider(this IRegistrationConfigurator configurator, string connectionString)
        {
            var martenRegistrationPipeline = configurator.AddMarten(
                options =>
                {
                    options.Connection(connectionString);
                });

#if NET6_0_OR_GREATER
            martenRegistrationPipeline.ApplyAllDatabaseChangesOnStartup();
#endif

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
            var martenRegistrationPipeline = configurator.AddMarten(
                options =>
                {
                    options.Connection(connectionString);

                    configureOptions?.Invoke(options);
                });

#if NET6_0_OR_GREATER
            martenRegistrationPipeline.ApplyAllDatabaseChangesOnStartup();
#endif

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
            var martenRegistrationPipeline = configurator.AddMarten(
                options =>
                {
                    options.Connection(connectionFactory);

                    configureOptions?.Invoke(options);
                });

#if NET6_0_OR_GREATER
            martenRegistrationPipeline.ApplyAllDatabaseChangesOnStartup();
#endif

            configurator.SetSagaRepositoryProvider(new MartenSagaRepositoryRegistrationProvider());
        }
    }
}
