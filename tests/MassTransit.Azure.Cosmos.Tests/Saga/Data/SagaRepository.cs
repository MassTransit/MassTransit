namespace MassTransit.Azure.Cosmos.Tests.Saga.Data
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using AzureCosmos;
    using AzureCosmos.Saga;
    using Microsoft.Azure.Cosmos;


    public sealed class SagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        static readonly SagaRepository<TSaga> _instance = new SagaRepository<TSaga>();

        public static readonly SagaRepository<TSaga> Instance = _instance;

        public static string DatabaseName = "sagaTest";
        public static string CollectionName = typeof(TSaga).Name;
        Container _container;

        Database _database;

        SagaRepository()
        {
            Client = new CosmosClient(Configuration.EndpointUri, Configuration.Key,
                new CosmosClientOptions { Serializer = new CosmosJsonDotNetSerializer(JsonSerializerSettingsExtensions.GetSagaRenameSettings<TSaga>()) });
        }

        public CosmosClient Client { get; }

        public async Task Initialize()
        {
            // Should all be part of the singleton initializer, because msft says it can take time the first connect...
            var databaseResponse = await Client.CreateDatabaseIfNotExistsAsync(DatabaseName).ConfigureAwait(false);
            var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(CollectionName, "/id");

            _database = databaseResponse.Database;
            _container = containerResponse.Container;
        }

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
                ItemResponse<TSaga> responseDoc = await _container.ReadItemAsync<TSaga>(correlationId.ToString(), new PartitionKey(correlationId.ToString()));

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
                ItemResponse<TSaga> responseDoc = await _container.ReadItemAsync<TSaga>(correlationId.ToString(), new PartitionKey(correlationId.ToString()));

                return responseDoc;
            }
            catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}
