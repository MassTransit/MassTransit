namespace MassTransit
{
    using System;
    using Marten;
    using Npgsql;


    public interface IMartenSagaRepositoryConfigurator
    {
        void Connection(string connectionString, Action<StoreOptions> configure = null);
        void Connection(Func<NpgsqlConnection> connectionFactory, Action<StoreOptions> configure = null);
    }


    public interface IMartenSagaRepositoryConfigurator<TSaga> :
        IMartenSagaRepositoryConfigurator
        where TSaga : class, ISaga
    {
    }
}
