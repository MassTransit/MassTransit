namespace MassTransit
{
    using System;
    using Marten;


    public interface IMartenSagaRepositoryConfigurator
    {
        [Obsolete("Use AddMarten to configure the connection. Visit https://masstransit.io/obsolete for details.")]
        void Connection(string connectionString, Action<StoreOptions> configure = null);
    }


    public interface IMartenSagaRepositoryConfigurator<TSaga> :
        IMartenSagaRepositoryConfigurator
        where TSaga : class, ISaga
    {
    }
}
