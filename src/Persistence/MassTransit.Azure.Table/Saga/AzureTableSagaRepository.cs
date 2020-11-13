namespace MassTransit.Azure.Table.Saga
{
    using System;
    using Contexts;
    using MassTransit.Saga;
    using Microsoft.Azure.Cosmos.Table;


    public static class AzureTableSagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        public static ISagaRepository<TSaga> Create(Func<CloudTable> tableFactory, ISagaKeyFormatter<TSaga> keyFormatter)
        {
            var consumeContextFactory = new SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>();

            var repositoryContextFactory = new AzureTableSagaRepositoryContextFactory<TSaga>(tableFactory, consumeContextFactory, keyFormatter);

            return new SagaRepository<TSaga>(repositoryContextFactory);
        }

        public static ISagaRepository<TSaga> Create(Func<CloudTable> tableFactory)
        {
            return Create(tableFactory, new ConstPartitionSagaKeyFormatter<TSaga>(typeof(TSaga).Name));
        }
    }
}
