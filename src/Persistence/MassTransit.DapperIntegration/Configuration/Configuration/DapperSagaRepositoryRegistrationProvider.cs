namespace MassTransit.Configuration
{
    using System;


    public class DapperSagaRepositoryRegistrationProvider :
        ISagaRepositoryRegistrationProvider
    {
        readonly Action<IDapperSagaRepositoryConfigurator> _configure;
        readonly string _connectionString;

        public DapperSagaRepositoryRegistrationProvider(string connectionString, Action<IDapperSagaRepositoryConfigurator> configure)
        {
            _connectionString = connectionString;
            _configure = configure;
        }

        public virtual void Configure<TSaga>(ISagaRegistrationConfigurator<TSaga> configurator)
            where TSaga : class, ISaga
        {
            configurator.DapperRepository(_connectionString, r => _configure?.Invoke(r));
        }
    }
}
