namespace MassTransit.Configuration
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using RedisIntegration.Saga;
    using Saga;
    using StackExchange.Redis;


    public class RedisSagaRepositoryConfigurator<TSaga> :
        IRedisSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, ISagaVersion
    {
        ConfigurationOptions _configurationOptions;
        Func<IServiceProvider, ConnectionMultiplexer> _connectionFactory;
        SelectDatabase _databaseSelector;

        public RedisSagaRepositoryConfigurator()
        {
            ConcurrencyMode = ConcurrencyMode.Optimistic;
            KeyPrefix = "";
            LockRetryTimeout = LockTimeout = TimeSpan.FromSeconds(30);

            DatabaseConfiguration("127.0.0.1");
            _databaseSelector = SelectDefaultDatabase;
        }

        public ConcurrencyMode ConcurrencyMode { get; set; }
        public string KeyPrefix { get; set; }
        public string LockSuffix { get; set; }
        public TimeSpan LockTimeout { get; set; }
        public TimeSpan LockRetryTimeout { get; set; }
        public TimeSpan? Expiry { get; set; }

        public void DatabaseConfiguration(string configuration)
        {
            DatabaseConfiguration(ConfigurationOptions.Parse(configuration));
        }

        public void DatabaseConfiguration(ConfigurationOptions configurationOptions)
        {
            _configurationOptions = configurationOptions;

            _connectionFactory = provider => ConnectionMultiplexer.Connect(_configurationOptions);
        }

        public void ConnectionFactory(Func<ConnectionMultiplexer> connectionFactory)
        {
            _connectionFactory = provider => connectionFactory();
        }

        public void ConnectionFactory(Func<IServiceProvider, ConnectionMultiplexer> connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void SelectDatabase(SelectDatabase databaseSelector)
        {
            _databaseSelector = databaseSelector;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_connectionFactory == null)
                yield return this.Failure("ConnectionFactory", "must be specified");
            if (LockTimeout <= TimeSpan.Zero)
                yield return this.Failure("LockTimeout", "Must be > TimeSpan.Zero");
            if (LockRetryTimeout <= TimeSpan.Zero)
                yield return this.Failure("LockRetryTimeout", "Must be > TimeSpan.Zero");
        }

        public void Register<T>(ISagaRepositoryRegistrationConfigurator<T> configurator)
            where T : class, ISagaVersion
        {
            configurator.TryAddSingleton(_connectionFactory);
            configurator.TryAddSingleton(new RedisSagaRepositoryOptions<T>(ConcurrencyMode, LockTimeout, LockSuffix, KeyPrefix, _databaseSelector, Expiry));
            configurator.RegisterSagaRepository<T, DatabaseContext<T>, SagaConsumeContextFactory<DatabaseContext<T>, T>,
                RedisSagaRepositoryContextFactory<T>>();
        }

        static IDatabase SelectDefaultDatabase(IConnectionMultiplexer multiplexer)
        {
            return multiplexer.GetDatabase();
        }
    }
}
