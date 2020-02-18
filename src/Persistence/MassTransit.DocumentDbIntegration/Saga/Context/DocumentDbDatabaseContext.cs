namespace MassTransit.DocumentDbIntegration.Saga.Context
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Newtonsoft.Json;


    public class DocumentDbDatabaseContext<TSaga> :
        DatabaseContext<TSaga>
        where TSaga : class, IVersionedSaga
    {
        readonly string _databaseId;
        readonly string _collectionId;
        readonly JsonSerializerSettings _serializerSettings;

        public DocumentDbDatabaseContext(IDocumentClient client, string databaseId, string collectionId, JsonSerializerSettings serializerSettings = null,
            FeedOptions feedOptions = null, RequestOptions requestOptions = null)
        {
            _databaseId = databaseId;
            _collectionId = collectionId;
            _serializerSettings = serializerSettings;

            Client = client;
            FeedOptions = feedOptions ?? new FeedOptions();
            RequestOptions = requestOptions ?? new RequestOptions();

            if (serializerSettings != null)
            {
                FeedOptions.JsonSerializerSettings = serializerSettings;
                RequestOptions.JsonSerializerSettings = serializerSettings;
            }

            Collection = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);
        }

        public IDocumentClient Client { get; }
        public Uri Collection { get; }
        public FeedOptions FeedOptions { get; }
        public RequestOptions RequestOptions { get; }

        public async Task<TSaga> Load(Guid correlationId, CancellationToken cancellationToken)
        {
            try
            {
                var documentUri = UriFactory.CreateDocumentUri(_databaseId, _collectionId, correlationId.ToString());

                ResourceResponse<Document> response = await Client.ReadDocumentAsync(documentUri, RequestOptions, cancellationToken)
                    .ConfigureAwait(false);

                return JsonConvert.DeserializeObject<TSaga>(response.Resource.ToString(), _serializerSettings);
            }
            catch (DocumentClientException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task Add(TSaga instance, CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new RequestOptions
                {
                    ConsistencyLevel = RequestOptions.ConsistencyLevel,
                    JsonSerializerSettings = _serializerSettings
                };

                await Client.CreateDocumentAsync(Collection, instance, requestOptions, true, cancellationToken).ConfigureAwait(false);
            }
            catch (DocumentClientException e) when (e.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                throw new DocumentDbConcurrencyException("Unable to update saga. It may not have been found or may have been updated by another process.");
            }
        }

        public async Task<TSaga> Insert(TSaga instance, CancellationToken cancellationToken)
        {
            try
            {
                var requestOptions = new RequestOptions
                {
                    ConsistencyLevel = RequestOptions.ConsistencyLevel,
                    JsonSerializerSettings = _serializerSettings
                };

                await Client.CreateDocumentAsync(Collection, instance, requestOptions, true, cancellationToken).ConfigureAwait(false);

                return instance;
            }
            catch (DocumentClientException)
            {
                throw new DocumentDbConcurrencyException("Unable to update saga. It may not have been found or may have been updated by another process.");
            }
        }

        public async Task Update(TSaga instance, CancellationToken cancellationToken)
        {
            // DocumentDb Optimistic Concurrency https://codeopinion.com/documentdb-optimistic-concurrency/
            var accessCondition = new AccessCondition
            {
                Condition = instance.ETag,
                Type = AccessConditionType.IfMatch
            };

            try
            {
                var requestOptions = new RequestOptions
                {
                    AccessCondition = accessCondition,
                    ConsistencyLevel = RequestOptions.ConsistencyLevel,
                    JsonSerializerSettings = _serializerSettings
                };

                var documentUri = UriFactory.CreateDocumentUri(_databaseId, _collectionId, instance.CorrelationId.ToString());

                await Client.ReplaceDocumentAsync(documentUri, instance, requestOptions, cancellationToken).ConfigureAwait(false);
            }
            catch (DocumentClientException e) when (e.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                throw new DocumentDbConcurrencyException("Unable to update saga. It may not have been found or may have been updated by another process.");
            }
        }

        public async Task Delete(TSaga instance, CancellationToken cancellationToken)
        {
            // DocumentDb Optimistic Concurrency https://codeopinion.com/documentdb-optimistic-concurrency/
            var accessCondition = new AccessCondition
            {
                Condition = instance.ETag,
                Type = AccessConditionType.IfMatch
            };

            try
            {
                var requestOptions = new RequestOptions
                {
                    AccessCondition = accessCondition,
                    ConsistencyLevel = RequestOptions.ConsistencyLevel,
                    JsonSerializerSettings = _serializerSettings
                };

                var documentUri = UriFactory.CreateDocumentUri(_databaseId, _collectionId, instance.CorrelationId.ToString());

                await Client.DeleteDocumentAsync(documentUri, requestOptions, cancellationToken).ConfigureAwait(false);
            }
            catch (DocumentClientException e) when (e.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                throw new DocumentDbConcurrencyException("Unable to update saga. It may not have been found or may have been updated by another process.");
            }
        }
    }
}
