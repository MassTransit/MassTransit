namespace MassTransit
{
    using System;
    using StackExchange.Redis;


    public interface IRedisSagaRepositoryConfigurator
    {
        ConcurrencyMode ConcurrencyMode { set; }

        string KeyPrefix { set; }

        string LockSuffix { set; }

        TimeSpan LockTimeout { set; }

        TimeSpan LockRetryTimeout { set; }

        TimeSpan? Expiry { set; }

        /// <summary>
        /// Set the database factory using configuration, which caches a <see cref="ConnectionMultiplexer" /> under the hood.
        /// </summary>
        /// <param name="configuration"></param>
        void DatabaseConfiguration(string configuration);

        /// <summary>
        /// Set the database factory using configuration, which caches a <see cref="ConnectionMultiplexer" /> under the hood.
        /// </summary>
        /// <param name="configurationOptions"></param>
        void DatabaseConfiguration(ConfigurationOptions configurationOptions);

        /// <summary>
        /// Use a simple factory method to create the connection
        /// </summary>
        /// <param name="connectionFactory"></param>
        void ConnectionFactory(Func<ConnectionMultiplexer> connectionFactory);

        /// <summary>
        /// Use the configuration service provider to resolve the connection
        /// </summary>
        /// <param name="connectionFactory"></param>
        void ConnectionFactory(Func<IServiceProvider, ConnectionMultiplexer> connectionFactory);

        /// <summary>
        /// Select a database other than the default to be used (optional)
        /// </summary>
        /// <param name="databaseSelector"></param>
        void SelectDatabase(SelectDatabase databaseSelector);
    }


    public interface IRedisSagaRepositoryConfigurator<TSaga> :
        IRedisSagaRepositoryConfigurator
        where TSaga : class, ISagaVersion
    {
    }
}
