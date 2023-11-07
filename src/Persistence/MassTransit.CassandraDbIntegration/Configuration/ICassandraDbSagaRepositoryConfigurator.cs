namespace MassTransit.Configuration
{
    using System;
    using Cassandra;


    public interface ICassandraDbSagaRepositoryConfigurator
    {
        string TableName { set; }

        /// <summary>
        /// Factory method to get the CassandraDb context
        /// </summary>
        /// <param name="contextFactory"></param>
        void ContextFactory(Func<ISession> contextFactory);

        /// <summary>
        /// Use the container to build the CassandraDb context
        /// </summary>
        /// <param name="contextFactory"></param>
        void ContextFactory(Func<IServiceProvider, ISession> contextFactory);
    }


    public interface ICassandraDbSagaRepositoryConfigurator<TSaga> :
        ICassandraDbSagaRepositoryConfigurator
        where TSaga : class, ISagaVersion
    {
    }
}
