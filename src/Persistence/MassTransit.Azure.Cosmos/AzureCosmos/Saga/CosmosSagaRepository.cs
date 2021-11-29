namespace MassTransit.AzureCosmos.Saga
{
    using System;
    using MassTransit.Saga;
    using Microsoft.Azure.Cosmos;


    public static class CosmosSagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        public static ISagaRepository<TSaga> Create(CosmosClient client, string databaseName)
        {
            return Create(client, databaseName, KebabCaseCollectionIdFormatter.Instance);
        }

        public static ISagaRepository<TSaga> Create(CosmosClient client, string databaseName, string collectionId)
        {
            if (string.IsNullOrWhiteSpace(collectionId))
                throw new ArgumentNullException(nameof(collectionId));

            return Create(client, databaseName, new DefaultCollectionIdFormatter(collectionId));
        }

        public static ISagaRepository<TSaga> Create(CosmosClient client, string databaseName, ICosmosCollectionIdFormatter collectionIdFormatter)
        {
            if (collectionIdFormatter == null)
                throw new ArgumentNullException(nameof(collectionIdFormatter));

            var databaseContext = new CosmosDatabaseContext<TSaga>(client.GetContainer(databaseName, collectionIdFormatter.Saga<TSaga>()));

            var consumeContextFactory = new SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>();

            var repositoryFactory = new CosmosSagaRepositoryContextFactory<TSaga>(databaseContext, consumeContextFactory);

            return new SagaRepository<TSaga>(repositoryFactory);
        }
    }
}
