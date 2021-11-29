namespace MassTransit
{
    using System;
    using Configuration;


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
            var repositoryConfigurator = new RedisSagaRepositoryConfigurator<T>();

            configure?.Invoke(repositoryConfigurator);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The Redis saga repository configuration is invalid:");

            configurator.Repository(x => repositoryConfigurator.Register(x));

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
            var repositoryConfigurator = new RedisSagaRepositoryConfigurator<T>();

            repositoryConfigurator.DatabaseConfiguration(configuration);

            configure?.Invoke(repositoryConfigurator);

            repositoryConfigurator.Validate().ThrowIfContainsFailure("The Redis saga repository configuration is invalid:");

            configurator.Repository(x => repositoryConfigurator.Register(x));

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
