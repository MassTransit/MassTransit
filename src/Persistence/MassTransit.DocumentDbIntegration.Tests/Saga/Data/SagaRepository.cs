namespace MassTransit.DocumentDbIntegration.Tests.Saga
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Newtonsoft.Json;


    public sealed class SagaRepository
    {
        static readonly SagaRepository _instance = new SagaRepository();

        private SagaRepository() { }

        public static SagaRepository Instance = _instance;

        public static string DatabaseName = "sagaTest";
        public static string CollectionName = "sagas";

        public async Task Initialize()
        {
            // Should all be part of the singleton initializer, because msft says it can take time the first connect...
            await _documentClient.OpenAsync();
            await _documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseName }).ConfigureAwait(false);
            await _documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DatabaseName), new DocumentCollection { Id = CollectionName }).ConfigureAwait(false);
        }

        private readonly DocumentClient _documentClient = new DocumentClient(new Uri(EmulatorConstants.EndpointUri), EmulatorConstants.Key);
        public IDocumentClient Client => _documentClient;

        public async Task InsertSaga(SimpleSaga saga)
        {
            await Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName), saga);
        }

        public async Task DeleteSaga(Guid correlationId)
        {
            try
            {
                await Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DatabaseName, CollectionName, correlationId.ToString()));
            }
            catch (DocumentClientException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Swallow not found
            }
        }

        public async Task<SimpleSaga> GetSaga(Guid correlationId)
        {
            try
            {
                var document = await Client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseName, CollectionName, correlationId.ToString()));
                return JsonConvert.DeserializeObject<SimpleSaga>(document.Resource.ToString());
            }
            catch (DocumentClientException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<Document> GetSagaDocument(Guid correlationId)
        {
            try
            {
                var document = await Client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DatabaseName, CollectionName, correlationId.ToString()));
                return document.Resource;
            }
            catch (DocumentClientException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}
