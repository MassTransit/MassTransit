namespace MassTransit.AzureTable.Saga
{
    using System;
    using Azure.Data.Tables;
    using MassTransit.Saga;


    public static class AzureTableSagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        public static ISagaRepository<TSaga> Create(Func<TableClient> tableFactory, ISagaKeyFormatter<TSaga> keyFormatter)
        {
            var consumeContextFactory = new SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>();

            var cloudTableProvider = new DelegateCloudTableProvider<TSaga>(tableFactory);

            var repositoryContextFactory = new AzureTableSagaRepositoryContextFactory<TSaga>(cloudTableProvider, consumeContextFactory, keyFormatter);

            return new SagaRepository<TSaga>(repositoryContextFactory, loadSagaRepositoryContextFactory: repositoryContextFactory);
        }

        public static ISagaRepository<TSaga> Create(Func<TableClient> tableFactory)
        {
            return Create(tableFactory, new ConstPartitionSagaKeyFormatter<TSaga>(typeof(TSaga).Name));
        }
    }
}
