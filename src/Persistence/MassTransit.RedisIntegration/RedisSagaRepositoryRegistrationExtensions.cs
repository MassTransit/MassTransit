namespace MassTransit
{
    using System;
    using Configurators;
    using RedisIntegration;
    using RedisIntegration.Configuration;
    using Saga;


    public static class RedisSagaRepositoryRegistrationExtensions
    {
        /// <summary>
        /// Adds a Redis saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<T> RedisRepository<T>(this ISagaRegistrationConfigurator<T> configurator,
            Action<IRedisSagaRepositoryConfigurator<T>> configure = null)
            where T : class, ISagaVersion
        {
            var redisConfigurator = new RedisSagaRepositoryConfigurator<T>();

            configure?.Invoke(redisConfigurator);

            BusConfigurationResult.CompileResults(redisConfigurator.Validate());

            configurator.Repository(x => redisConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Adds a Redis saga repository to the registration
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configuration">The Redis configuration string</param>
        /// <param name="configure"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ISagaRegistrationConfigurator<T> RedisRepository<T>(this ISagaRegistrationConfigurator<T> configurator, string configuration,
            Action<IRedisSagaRepositoryConfigurator<T>> configure = null)
            where T : class, ISagaVersion
        {
            var redisConfigurator = new RedisSagaRepositoryConfigurator<T>();

            redisConfigurator.DatabaseConfiguration(configuration);

            configure?.Invoke(redisConfigurator);

            BusConfigurationResult.CompileResults(redisConfigurator.Validate());

            configurator.Repository(x => redisConfigurator.Register(x));

            return configurator;
        }

        /// <summary>
        /// Use the Redis saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        public static void SetRedisSagaRepositoryProvider(this IRegistrationConfigurator configurator, Action<IRedisSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new RedisSagaRepositoryRegistrationProvider(configure));
        }

        /// <summary>
        /// Use the Redis saga repository for sagas configured by type (without a specific generic call to AddSaga/AddSagaStateMachine)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="configuration">The Redis configuration string</param>
        /// <param name="configure"></param>
        public static void SetRedisSagaRepositoryProvider(this IRegistrationConfigurator configurator, string configuration,
            Action<IRedisSagaRepositoryConfigurator> configure)
        {
            configurator.SetSagaRepositoryProvider(new RedisSagaRepositoryRegistrationProvider(r =>
            {
                r.DatabaseConfiguration(configuration);

                configure?.Invoke(r);
            }));
        }
    }
}
