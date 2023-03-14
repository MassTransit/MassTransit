namespace MassTransit
{
    using System;
    using Marten;
    using Npgsql;


    public interface IMartenSagaRepositoryConfigurator
    {
        [Obsolete("Use AddMarten to configure Marten connection details/options")]
        void Connection(string connectionString, Action<StoreOptions> configure = null);

        [Obsolete("Use AddMarten to configure Marten connection details/options")]
        void Connection(Func<NpgsqlConnection> connectionFactory, Action<StoreOptions> configure = null);
    }


    public interface IMartenSagaRepositoryConfigurator<TSaga> :
        IMartenSagaRepositoryConfigurator
        where TSaga : class, ISaga
    {
    }
}
