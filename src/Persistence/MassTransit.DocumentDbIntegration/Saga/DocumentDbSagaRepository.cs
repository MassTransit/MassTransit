namespace MassTransit.DocumentDbIntegration.Saga
{
    using System;
    using Context;
    using MassTransit.Saga;
    using Microsoft.Azure.Documents;
    using Newtonsoft.Json;


    public static class DocumentDbSagaRepository<TSaga>
        where TSaga : class, IVersionedSaga
    {
        const string DefaultCollectionName = "sagas";

        public static ISagaRepository<TSaga> Create(IDocumentClient client, string databaseName, JsonSerializerSettings serializerSettings = null)
        {
            return Create(client, databaseName, DefaultCollectionName, serializerSettings);
        }

        public static ISagaRepository<TSaga> Create(IDocumentClient client, string databaseName, string collectionName,
            JsonSerializerSettings serializerSettings = null)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException(nameof(collectionName));

            if (collectionName.Length > 120)
                throw new ArgumentException("Collection names must be no longer than 120 characters", nameof(collectionName));

            var databaseContext = new DocumentDbDatabaseContext<TSaga>(client, databaseName, collectionName ?? DefaultCollectionName, serializerSettings);

            var consumeContextFactory = new SagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga>();

            var repositoryFactory = new DocumentDbSagaRepositoryContextFactory<TSaga>(databaseContext, consumeContextFactory);

            return new SagaRepository<TSaga>(repositoryFactory);
        }
    }
}
