// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.DocumentDbIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using GreenPipes.Internals.Extensions;
    using Logging;
    using MassTransit.Saga;
    using Pipeline;
    using Util;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents;
    using Newtonsoft.Json;


    public class DocumentDbSagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>
        where TSaga : class, IVersionedSaga
    {
        public const string DefaultCollectionName = "sagas";
        private static readonly ILog _log = Logger.Get<DocumentDbSagaRepository<TSaga>>();

        private readonly IDocumentClient _client;
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly IDocumentDbSagaConsumeContextFactory _documentDbSagaConsumeContextFactory;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly FeedOptions _feedOptions;
        private readonly RequestOptions _requestOptions;

        public DocumentDbSagaRepository(IDocumentClient client, string databaseName, string collectionName)
            : this(client, databaseName, collectionName, new DocumentDbSagaConsumeContextFactory(), null, null)
        {
        }

        public DocumentDbSagaRepository(IDocumentClient client, string databaseName, IDocumentDbSagaConsumeContextFactory documentDbSagaConsumeContextFactory)
            : this(client, databaseName, DefaultCollectionName, documentDbSagaConsumeContextFactory, null, null)
        {
        }

        public DocumentDbSagaRepository(IDocumentClient client, string databaseName)
            : this(client, databaseName, DefaultCollectionName, new DocumentDbSagaConsumeContextFactory(), null, null)
        {
        }

        public DocumentDbSagaRepository(IDocumentClient client, string databaseName, JsonSerializerSettings jsonSerializerSettings)
            : this(client, databaseName, DefaultCollectionName, new DocumentDbSagaConsumeContextFactory(), jsonSerializerSettings)
        {
        }

        public DocumentDbSagaRepository(
            IDocumentClient client,
            string databaseName,
            string collectionName,
            IDocumentDbSagaConsumeContextFactory documentDbSagaConsumeContextFactory,
            JsonSerializerSettings jsonSerializerSettings)
            : this(client, databaseName, collectionName, documentDbSagaConsumeContextFactory, null, null)
        {
            if(jsonSerializerSettings != null)
            {
                _jsonSerializerSettings = jsonSerializerSettings;
                _requestOptions.JsonSerializerSettings = jsonSerializerSettings;
                _feedOptions.JsonSerializerSettings = jsonSerializerSettings;
            }
        }

        public DocumentDbSagaRepository(
            IDocumentClient client,
            string databaseName,
            string collectionName,
            IDocumentDbSagaConsumeContextFactory documentDbSagaConsumeContextFactory,
            FeedOptions feedOptions,
            RequestOptions requestOptions)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException(nameof(collectionName));

            if (collectionName.Length > 120)
                throw new ArgumentException("Collection names must be no longer than 120 characters", nameof(collectionName));

            _client = client;

            _databaseName = databaseName;
            _collectionName = collectionName;
            _documentDbSagaConsumeContextFactory = documentDbSagaConsumeContextFactory ?? throw new ArgumentNullException(nameof(documentDbSagaConsumeContextFactory));

            _feedOptions = feedOptions ?? new FeedOptions();
            _requestOptions = requestOptions ?? new RequestOptions();
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("sagaRepository");

            scope.Set(new
            {
                Persistence = "documentdb",
                SagaType = TypeMetadataCache<TSaga>.ShortName,
                Properties = TypeCache<TSaga>.ReadWritePropertyCache.Select(x => x.Property.Name).ToArray()
            });
        }

        public async Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            if (!context.CorrelationId.HasValue)
                throw new SagaException("The CorrelationId was not specified", typeof(TSaga), typeof(T));

            TSaga instance = null;

            if (policy.PreInsertInstance(context, out instance))
            {
                instance = await PreInsertSagaInstance(context, instance).ConfigureAwait(false);
            }

            if (instance == null)
            {
                try
                {
                    ResourceResponse<Document> response = await _client
                        .ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseName, _collectionName, context.CorrelationId.ToString()), _requestOptions)
                        .ConfigureAwait(false);

                    instance = JsonConvert.DeserializeObject<TSaga>(response.Resource.ToString(), _jsonSerializerSettings);
                }
                catch (DocumentClientException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Couldn't find the document, swallowing exception
                }
            }

            if (instance == null)
            {
                var missingSagaPipe =
                    new MissingPipe<TSaga, T>(_client, _databaseName, _collectionName, next, _documentDbSagaConsumeContextFactory, _jsonSerializerSettings);

                await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
            }
            else
            {
                await SendToInstance(context, policy, next, instance).ConfigureAwait(false);
            }
        }

        public async Task SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            try
            {
                var sagaInstances = (await _client
                    .CreateDocumentQuery<TSaga>(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), _feedOptions)
                    .Where(context.Query.FilterExpression).QueryAsync().ConfigureAwait(false)).ToList();

                if (!sagaInstances.Any())
                {
                    var missingPipe = new MissingPipe<TSaga, T>(_client, _databaseName, _collectionName, next, _documentDbSagaConsumeContextFactory, _jsonSerializerSettings);

                    await policy.Missing(context, missingPipe).ConfigureAwait(false);
                }
                else
                {
                    foreach (var instance in sagaInstances)
                    {
                        await SendToInstance(context, policy, next, instance).ConfigureAwait(false);
                    }
                }
            }
            catch (SagaException sex)
            {
                if (_log.IsErrorEnabled)
                    _log.Error($"SAGA:{TypeMetadataCache<TSaga>.ShortName} Exception {TypeMetadataCache<T>.ShortName}", sex);

                throw;
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                    _log.Error($"SAGA:{TypeMetadataCache<TSaga>.ShortName} Exception {TypeMetadataCache<T>.ShortName}", ex);

                throw new SagaException(ex.Message, typeof(TSaga), typeof(T), Guid.Empty, ex);
            }
        }

        async Task<TSaga> PreInsertSagaInstance<T>(ConsumeContext<T> context, TSaga instance)
            where T : class
        {
            try
            {
                ResourceResponse<Document> response =
                    await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), instance, _requestOptions, true).ConfigureAwait(false);

                _log.DebugFormat("SAGA:{0}:{1} Insert {2}", TypeMetadataCache<TSaga>.ShortName, instance.CorrelationId, TypeMetadataCache<T>.ShortName);

                return JsonConvert.DeserializeObject<TSaga>(response.Resource.ToString(), _jsonSerializerSettings);
            }
            catch (Exception ex)
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("SAGA:{0}:{1} Dupe {2} - {3}", TypeMetadataCache<TSaga>.ShortName, instance.CorrelationId, TypeMetadataCache<T>.ShortName,
                        ex.Message);
            }

            return null;
        }

        async Task SendToInstance<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next, TSaga instance)
            where T : class
        {
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("SAGA:{0}:{1} Used {2}", TypeMetadataCache<TSaga>.ShortName, instance.CorrelationId, TypeMetadataCache<T>.ShortName);

                SagaConsumeContext<TSaga, T> sagaConsumeContext =
                    _documentDbSagaConsumeContextFactory.Create(_client, _databaseName, _collectionName, context, instance, true, _requestOptions);

                await policy.Existing(sagaConsumeContext, next).ConfigureAwait(false);

                if (!sagaConsumeContext.IsCompleted)
                    await UpdateDocumentDbSaga(context, instance).ConfigureAwait(false);
            }
            catch (SagaException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SagaException(ex.Message, typeof(TSaga), typeof(T), instance.CorrelationId, ex);
            }
        }

        async Task UpdateDocumentDbSaga(PipeContext context, TSaga instance)
        {
            // DocumentDb Optimistic Concurrency https://codeopinion.com/documentdb-optimistic-concurrency/
            var ac = new AccessCondition {Condition = instance.ETag, Type = AccessConditionType.IfMatch};

            try
            {
                _requestOptions.AccessCondition = ac;
                await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_databaseName, _collectionName, instance.CorrelationId.ToString()), instance,
                    _requestOptions).ConfigureAwait(false);
            }
            catch (DocumentClientException e) when (e.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
            {
                throw new DocumentDbConcurrencyException("Unable to update saga. It may not have been found or may have been updated by another process.");
            }
            finally
            {
                _requestOptions.AccessCondition = null; // Unassign
            }
        }

        public async Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
        {
            // This will not work for Document Db because the .Where needs to look for [JsonProperty("id")], and if you pass in CorrelationId property, the ISaga doesn't have that property.
            IEnumerable<TSaga> sagas = await _client.CreateDocumentQuery<TSaga>(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), _feedOptions)
                .Where(query.FilterExpression)
                .QueryAsync()
                .ConfigureAwait(false);

            return sagas.Select(x => x.CorrelationId);
        }
    }
}