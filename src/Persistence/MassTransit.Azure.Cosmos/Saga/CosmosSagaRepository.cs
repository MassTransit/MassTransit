namespace MassTransit.Azure.Cosmos.Saga
{
    using System;
    using Context;
    using MassTransit.Saga;
    using Microsoft.Azure.Cosmos;


    public static class CosmosSagaRepository<TSaga>
        where TSaga : class, IVersionedSaga
    {
        const string DefaultCollectionName = "sagas";

        public static ISagaRepository<TSaga> Create(CosmosClient client, string databaseName)
        {
            return Create(client, databaseName, DefaultCollectionName);
        }

        public static ISagaRepository<TSaga> Create(CosmosClient client, string databaseName, string collectionName)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException(nameof(collectionName));

            if (collectionName.Length > 120)
                throw new ArgumentException("Collection names must be no longer than 120 characters", nameof(collectionName));

            var databaseContext = new CosmosDatabaseContext<TSaga>(client, databaseName, collectionName ?? DefaultCollectionName);

            var consumeContextFactory = new SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>();

            var repositoryFactory = new CosmosSagaRepositoryContextFactory<TSaga>(databaseContext, consumeContextFactory);

            return new SagaRepository<TSaga>(repositoryFactory);
        }
    }
}
