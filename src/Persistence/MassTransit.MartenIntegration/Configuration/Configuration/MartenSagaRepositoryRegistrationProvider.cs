namespace MassTransit.Configuration
{
    using System;
    using Marten;
    using Npgsql;


    public class MartenSagaRepositoryRegistrationProvider :
        ISagaRepositoryRegistrationProvider
    {
        readonly Action<StoreOptions> _configure;
        readonly Func<NpgsqlConnection> _connectionFactory;
        readonly string _connectionString;

        public MartenSagaRepositoryRegistrationProvider(string connectionString, Action<StoreOptions> configure)
        {
            _connectionString = connectionString;
            _configure = configure;
        }

        public MartenSagaRepositoryRegistrationProvider(Func<NpgsqlConnection> connectionFactory, Action<StoreOptions> configure)
        {
            _connectionFactory = connectionFactory;
            _configure = configure;
        }

        public virtual void Configure<TSaga>(ISagaRegistrationConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            if (_connectionFactory != null)
                configurator.MartenRepository(_connectionFactory, options => _configure?.Invoke(options));
            else
                configurator.MartenRepository(_connectionString, options => _configure?.Invoke(options));
        }
    }
}
