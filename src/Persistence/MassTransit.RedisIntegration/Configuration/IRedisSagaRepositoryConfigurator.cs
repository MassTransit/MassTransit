namespace MassTransit.RedisIntegration
{
    using System;
    using Registration;
    using StackExchange.Redis;


    public interface IRedisSagaRepositoryConfigurator
    {
        ConcurrencyMode ConcurrencyMode { set; }

        string KeyPrefix { set; }

        TimeSpan LockTimeout { set; }

        TimeSpan LockRetryTimeout { set; }

        /// <summary>
        /// Set the database factory using configuration, which caches a <see cref="ConnectionMultiplexer"/> under the hood.
        /// </summary>
        /// <param name="configuration"></param>
        void DatabaseConfiguration(string configuration);

        /// <summary>
        /// Set the database factory using configuration, which caches a <see cref="ConnectionMultiplexer"/> under the hood.
        /// </summary>
        /// <param name="configurationOptions"></param>
        void DatabaseConfiguration(ConfigurationOptions configurationOptions);

        /// <summary>
        /// Use a simple factory method to create the database
        /// </summary>
        /// <param name="databaseFactory"></param>
        void DatabaseFactory(Func<IDatabase> databaseFactory);

        /// <summary>
        /// Use the configuration service provider to resolve the database factory
        /// </summary>
        /// <param name="databaseFactory"></param>
        void DatabaseFactory(Func<IConfigurationServiceProvider, Func<IDatabase>> databaseFactory);
    }


    public interface IRedisSagaRepositoryConfigurator<TSaga> :
        IRedisSagaRepositoryConfigurator
        where TSaga : class, IVersionedSaga
    {
    }
}
