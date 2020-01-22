namespace MassTransit
{
    using System;
    using Configurators;
    using Marten;
    using MartenIntegration;
    using MartenIntegration.Configurators;
    using Saga;


    public static class MartenSagaRepositoryRegistrationExtensions
    {
        /// <summary>
        /// Adds a Redis saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<T> MartenRepository<T>(this ISagaRegistrationConfigurator<T> configurator,
            Action<IMartenSagaRepositoryConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            var redisConfigurator = new MartenSagaRepositoryConfigurator<T>();

            configure?.Invoke(redisConfigurator);

            BusConfigurationResult.CompileResults(redisConfigurator.Validate());

            configurator.Repository(x => redisConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Adds a Marten saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString">The Marten configuration string</param>
        /// <param name="configureOptions"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<T> MartenRepository<T>(this ISagaRegistrationConfigurator<T> configurator, string connectionString,
            Action<StoreOptions> configureOptions,
            Action<IMartenSagaRepositoryConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            var redisConfigurator = new MartenSagaRepositoryConfigurator<T>();

            redisConfigurator.Connection(connectionString, configureOptions);

            configure?.Invoke(redisConfigurator);

            BusConfigurationResult.CompileResults(redisConfigurator.Validate());

            configurator.Repository(x => redisConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Adds a Marten saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="connectionString">The Marten configuration string</param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<T> MartenRepository<T>(this ISagaRegistrationConfigurator<T> configurator, string connectionString,
            Action<IMartenSagaRepositoryConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            var redisConfigurator = new MartenSagaRepositoryConfigurator<T>();

            redisConfigurator.Connection(connectionString);

            configure?.Invoke(redisConfigurator);

            BusConfigurationResult.CompileResults(redisConfigurator.Validate());

            configurator.Repository(x => redisConfigurator.Register(x));

            return configurator;
        }
    }
}
