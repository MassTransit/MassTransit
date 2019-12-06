namespace MassTransit.RedisIntegration
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using Registration;
    using Saga;
    using StackExchange.Redis;


    public class RedisSagaRepositoryConfigurator<TSaga> :
        IRedisSagaRepositoryConfigurator<TSaga>,
        ISpecification
        where TSaga : class, IVersionedSaga
    {
        Func<IConfigurationServiceProvider, Func<IDatabase>> _databaseFactory;
        ConfigurationOptions _configurationOptions;

        public RedisSagaRepositoryConfigurator()
        {
            ConcurrencyMode = ConcurrencyMode.Optimistic;
            KeyPrefix = "";
            LockRetryTimeout = LockTimeout = TimeSpan.FromSeconds(30);
        }

        public ConcurrencyMode ConcurrencyMode { get; set; }
        public string KeyPrefix { get; set; }
        public TimeSpan LockTimeout { get; set; }
        public TimeSpan LockRetryTimeout { get; set; }

        public void DatabaseConfiguration(string configuration)
        {
            DatabaseConfiguration(ConfigurationOptions.Parse(configuration));
        }

        public void DatabaseConfiguration(ConfigurationOptions configurationOptions)
        {
            _configurationOptions = configurationOptions;

            _databaseFactory = provider =>
            {
                // TODO: This needs to be disposed when the container is disposed, so how do we do that!!
                var connection = ConnectionMultiplexer.Connect(_configurationOptions);
                connection.PreserveAsyncOrder = false;

                return () => connection.GetDatabase();
            };
        }

        public void DatabaseFactory(Func<IDatabase> databaseFactory)
        {
            _databaseFactory = provider => databaseFactory;
        }

        public void DatabaseFactory(Func<IConfigurationServiceProvider, Func<IDatabase>> databaseFactory)
        {
            _databaseFactory = databaseFactory;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_databaseFactory == null)
                yield return this.Failure("DatabaseFactory", "must be specified");
            if (LockTimeout <= TimeSpan.Zero)
                yield return this.Failure("LockTimeout", "Must be > TimeSpan.Zero");
            if (LockRetryTimeout <= TimeSpan.Zero)
                yield return this.Failure("LockRetryTimeout", "Must be > TimeSpan.Zero");
        }

        public Func<IConfigurationServiceProvider, ISagaRepository<TSaga>> BuildFactoryMethod()
        {
            ISagaRepository<TSaga> CreateRepository(IConfigurationServiceProvider provider)
            {
                var databaseFactory = _databaseFactory(provider);
                var optimistic = ConcurrencyMode == ConcurrencyMode.Optimistic;

                return new RedisSagaRepository<TSaga>(databaseFactory, optimistic, LockTimeout, LockRetryTimeout, KeyPrefix);
            }

            return CreateRepository;
        }
    }
}
