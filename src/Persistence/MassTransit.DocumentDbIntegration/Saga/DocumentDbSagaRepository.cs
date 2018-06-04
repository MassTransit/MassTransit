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
        where TSaga : class, ISaga
    {
        public const string DefaultCollectionName = "sagas";
        static readonly ILog _log = Logger.Get<DocumentDbSagaRepository<TSaga>>();

        static readonly RequestOptions _requestOptions;
        static readonly FeedOptions _feedOptions;
        static readonly JsonSerializerSettings _jsonSerializerSettings;

        static DocumentDbSagaRepository()
        {
            var resolver = new PropertyRenameSerializerContractResolver();
            resolver.RenameProperty(typeof(TSaga), "CorrelationId", "id");

            _jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = resolver
            };

            _requestOptions = new RequestOptions {JsonSerializerSettings = _jsonSerializerSettings};

            _feedOptions = new FeedOptions {JsonSerializerSettings = _jsonSerializerSettings};
        }

        readonly IDocumentClient _client;
        readonly string _databaseName;
        readonly string _collectionName;
        readonly IDocumentDbSagaConsumeContextFactory _documentDbSagaConsumeContextFactory;

        public DocumentDbSagaRepository(IDocumentClient client, string databaseName, string collectionName)
            : this(client, databaseName, collectionName, new DocumentDbSagaConsumeContextFactory())
        {
        }

        public DocumentDbSagaRepository(IDocumentClient client, string databaseName, IDocumentDbSagaConsumeContextFactory documentDbSagaConsumeContextFactory)
            : this(client, databaseName, DefaultCollectionName, documentDbSagaConsumeContextFactory)
        {
        }

        public DocumentDbSagaRepository(IDocumentClient client, string databaseName)
            : this(client, databaseName, DefaultCollectionName, new DocumentDbSagaConsumeContextFactory())
        {
        }

        public DocumentDbSagaRepository(IDocumentClient client, string databaseName, string collectionName,
            IDocumentDbSagaConsumeContextFactory documentDbSagaConsumeContextFactory)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentNullException(nameof(collectionName));

            if (collectionName.Length > 120)
                throw new ArgumentException("Collection names must be no longer than 120 characters", nameof(collectionName));

            _client = client;

            _databaseName = databaseName;
            _collectionName = collectionName;
            _documentDbSagaConsumeContextFactory = documentDbSagaConsumeContextFactory;
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

            Document document = null;

            if (policy.PreInsertInstance(context, out var instance))
            {
                document = await PreInsertSagaInstance(context, instance).ConfigureAwait(false);
            }

            if (instance == null)
            {
                try
                {
                    ResourceResponse<Document> response = await _client
                        .ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseName, _collectionName, context.CorrelationId.ToString()), _requestOptions)
                        .ConfigureAwait(false);

                    document = response.Resource;
                }
                catch (DocumentClientException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // Couldn't find the document, swallowing exception
                }

                if (document != null)
                {
                    instance = JsonConvert.DeserializeObject<TSaga>(document.ToString());
                }
            }

            if (instance == null)
            {
                var missingSagaPipe =
                    new MissingPipe<TSaga, T>(_client, _databaseName, _collectionName, next, _documentDbSagaConsumeContextFactory, _requestOptions);

                await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
            }
            else
            {
                await SendToInstance(context, policy, next, instance, document).ConfigureAwait(false);
            }
        }

        public async Task SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            try
            {
                List<TSaga> sagaInstances = (await _client
                    .CreateDocumentQuery<TSaga>(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), _feedOptions)
                    .Where(context.Query.FilterExpression).QueryAsync().ConfigureAwait(false)).ToList();

                if (!sagaInstances.Any())
                {
                    var missingPipe = new MissingPipe<TSaga, T>(_client, _databaseName, _collectionName, next, _documentDbSagaConsumeContextFactory, _requestOptions);

                    await policy.Missing(context, missingPipe).ConfigureAwait(false);
                }
                else
                {
                    foreach (var instance in sagaInstances)
                    {
                        // To support optimistic concurrency, we need the Document ETag
                        // So we will re-query to get the document, this should be fast because we are querying by Id
                        var response = await _client
                            .ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseName, _collectionName, instance.CorrelationId.ToString()), _requestOptions)
                            .ConfigureAwait(false);

                        await SendToInstance(context, policy, next, instance, response.Resource).ConfigureAwait(false);
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

        async Task<Document> PreInsertSagaInstance<T>(ConsumeContext<T> context, TSaga instance)
            where T : class
        {
            try
            {
                ResourceResponse<Document> response =
                    await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), instance,
                        disableAutomaticIdGeneration: true, options: _requestOptions).ConfigureAwait(false);

                _log.DebugFormat("SAGA:{0}:{1} Insert {2}", TypeMetadataCache<TSaga>.ShortName, instance.CorrelationId, TypeMetadataCache<T>.ShortName);

                return response.Resource;
            }
            catch (Exception ex)
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("SAGA:{0}:{1} Dupe {2} - {3}", TypeMetadataCache<TSaga>.ShortName, instance.CorrelationId, TypeMetadataCache<T>.ShortName,
                        ex.Message);
            }

            return null;
        }

        async Task SendToInstance<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next, TSaga instance,
            Document document)
            where T : class
        {
            try
            {
                if (_log.IsDebugEnabled)
                    _log.DebugFormat("SAGA:{0}:{1} Used {2}", TypeMetadataCache<TSaga>.ShortName, instance.CorrelationId, TypeMetadataCache<T>.ShortName);

                SagaConsumeContext<TSaga, T> sagaConsumeContext =
                    _documentDbSagaConsumeContextFactory.Create(_client, _databaseName, _collectionName, context, instance);

                await policy.Existing(sagaConsumeContext, next).ConfigureAwait(false);

                if (!sagaConsumeContext.IsCompleted)
                    await UpdateDocumentDbSaga(context, instance, document).ConfigureAwait(false);
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

        async Task UpdateDocumentDbSaga(PipeContext context, TSaga instance, Document document)
        {
            if (document == null)
                throw new DocumentDbConcurrencyException("Document didn't pre-exist, shouldn't have reached here.");

            // DocumentDb Optimistic Concurrency https://codeopinion.com/documentdb-optimistic-concurrency/

            var ac = new AccessCondition {Condition = document.ETag, Type = AccessConditionType.IfMatch};

            try
            {
                await _client.ReplaceDocumentAsync(document.SelfLink, instance,
                    new RequestOptions {AccessCondition = ac, JsonSerializerSettings = _jsonSerializerSettings }).ConfigureAwait(false);
            }
            catch (DocumentClientException e) when (e.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
            {
                throw new DocumentDbConcurrencyException("Unable to update saga. It may not have been found or may have been updated by another process.");
            }
        }

        public async Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
        {
            IEnumerable<TSaga> sagas = await _client.CreateDocumentQuery<TSaga>(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), _feedOptions)
                .Where(query.FilterExpression)
                .QueryAsync()
                .ConfigureAwait(false);

            return sagas.Select(x => x.CorrelationId);
        }
    }
}