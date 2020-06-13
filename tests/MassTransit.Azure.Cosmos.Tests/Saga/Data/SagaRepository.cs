namespace MassTransit.Azure.Cosmos.Tests.Saga.Data
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;
    using Newtonsoft.Json;


    public sealed class SagaRepository<TSaga>
            where TSaga : class, IVersionedSaga
    {
        static readonly SagaRepository<TSaga> _instance = new SagaRepository<TSaga>();

        SagaRepository()
        {
        }

        public static readonly SagaRepository<TSaga> Instance = _instance;

        public static string DatabaseName = "sagaTest";
        public static string CollectionName = typeof(TSaga).Name;

        public async Task Initialize()
        {
            // Should all be part of the singleton initializer, because msft says it can take time the first connect...
            var databaseResponse = await Client.CreateDatabaseIfNotExistsAsync(DatabaseName).ConfigureAwait(false);
            var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(CollectionName, "/id");

            _database = databaseResponse.Database;
            _container = containerResponse.Container;
        }

        public CosmosClient Client { get; } = new CosmosClient(Configuration.EndpointUri, Configuration.Key, new CosmosClientOptions { Serializer = new CosmosJsonDotNetSerializer(JsonSerializerSettingsExtensions.GetSagaRenameSettings<TSaga>()) });
        private Database _database;
        private Container _container;

        public async Task<ItemResponse<TSaga>> InsertSaga(TSaga saga)
        {
            return await _container.CreateItemAsync(saga);
        }

        public async Task DeleteSaga(Guid correlationId)
        {
            try
            {
                await _container.DeleteItemAsync<TSaga>(correlationId.ToString(), new PartitionKey(correlationId.ToString()));
            }
            catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                // Swallow not found
            }
        }

        public async Task<TSaga> GetSaga(Guid correlationId)
        {
            try
            {
                var responseDoc = await _container.ReadItemAsync<TSaga>(correlationId.ToString(), new PartitionKey(correlationId.ToString()));

                return responseDoc.Resource;
            }
            catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<ItemResponse<TSaga>> GetSagaDocument(Guid correlationId)
        {
            try
            {
                var responseDoc = await _container.ReadItemAsync<TSaga>(correlationId.ToString(), new PartitionKey(correlationId.ToString()));

                return responseDoc;
            }
            catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}
