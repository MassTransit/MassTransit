namespace MassTransit.MartenIntegration
{
    using System;
    using Marten;
    using MassTransit.Saga;
    using Npgsql;


    public interface IMartenSagaRepositoryConfigurator<TSaga>
        where TSaga : class, ISaga
    {
        void Connection(string connectionString, Action<StoreOptions> configure = null);
        void Connection(Func<NpgsqlConnection> connectionFactory, Action<StoreOptions> configure = null);
    }
}
