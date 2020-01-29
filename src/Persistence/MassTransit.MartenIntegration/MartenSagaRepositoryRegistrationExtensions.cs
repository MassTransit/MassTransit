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
            var redisConfigurator = new MartenSagaRepositoryConfigurator<T>();

            redisConfigurator.Connection(connectionString, configureOptions);

            BusConfigurationResult.CompileResults(redisConfigurator.Validate());

            configurator.Repository(x => redisConfigurator.Register(x));

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
            var redisConfigurator = new MartenSagaRepositoryConfigurator<T>();

            redisConfigurator.Connection(connectionFactory, configureOptions);

            BusConfigurationResult.CompileResults(redisConfigurator.Validate());

            configurator.Repository(x => redisConfigurator.Register(x));

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
            var redisConfigurator = new MartenSagaRepositoryConfigurator<T>();

            redisConfigurator.Connection(connectionString);

            BusConfigurationResult.CompileResults(redisConfigurator.Validate());

            configurator.Repository(x => redisConfigurator.Register(x));

            return configurator;
        }
    }
}
