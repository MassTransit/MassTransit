namespace MassTransit.CassandraDbIntegration.Saga
{
    using System;
    using Cassandra;
    using MassTransit.Saga;


    public static class CassandraDbSagaRepository<TSaga>
        where TSaga : class, ISagaVersion
    {
        public static ISagaRepository<TSaga> Create(Func<ISession> cassandraDbFactory)
        {
            var options = new CassandraDbSagaRepositoryOptions<TSaga>();

            var consumeContextFactory = new SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>();

            var repositoryContextFactory = new CassandraDbSagaRepositoryContextFactory<TSaga>(cassandraDbFactory, consumeContextFactory, options);

            return new SagaRepository<TSaga>(repositoryContextFactory, loadSagaRepositoryContextFactory: repositoryContextFactory);
        }
    }
}
