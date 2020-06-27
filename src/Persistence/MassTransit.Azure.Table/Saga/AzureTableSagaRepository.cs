namespace MassTransit.Azure.Table.Saga
{
    using System;
    using Contexts;
    using MassTransit.Saga;
    using Microsoft.Azure.Cosmos.Table;


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
