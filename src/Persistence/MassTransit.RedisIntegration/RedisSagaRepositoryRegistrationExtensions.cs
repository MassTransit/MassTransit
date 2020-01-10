namespace MassTransit
{
    using System;
    using Configurators;
    using RedisIntegration;
    using RedisIntegration.Configuration;


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
            where T : class, IVersionedSaga
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
            where T : class, IVersionedSaga
        {
            var redisConfigurator = new RedisSagaRepositoryConfigurator<T>();

            redisConfigurator.DatabaseConfiguration(configuration);

            configure?.Invoke(redisConfigurator);

            BusConfigurationResult.CompileResults(redisConfigurator.Validate());

            configurator.Repository(x => redisConfigurator.Register(x));

            return configurator;
        }
    }
}
