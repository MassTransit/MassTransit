namespace MassTransit.Azure.Table
{
    using System;
    using Microsoft.Azure.Cosmos.Table;
    using Saga;


    public static class AzureTableSagaRepository<TSaga>
        where TSaga : class, IVersionedSaga
    {
        public static ISagaRepository<TSaga> Create(Func<CloudTable> tableFactory)
        {
            var consumeContextFactory = new SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>();

            var repositoryContextFactory = new AzureTableSagaRepositoryContextFactory<TSaga>(tableFactory, consumeContextFactory);

            return new SagaRepository<TSaga>(repositoryContextFactory);
        }
    }
}
