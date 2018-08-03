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
namespace MassTransit.DocumentDbIntegration.Saga.Pipeline
{
    using Context;
    using GreenPipes;
    using Logging;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Newtonsoft.Json;
    using System.Threading.Tasks;
    using Util;

    public class MissingPipe<TSaga, TMessage> :
        IPipe<SagaConsumeContext<TSaga, TMessage>>
        where TSaga : class, IVersionedSaga
        where TMessage : class
    {
        static readonly ILog _log = Logger.Get<DocumentDbSagaRepository<TSaga>>();
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly IDocumentClient _client;
        private readonly IDocumentDbSagaConsumeContextFactory _documentDbSagaConsumeContextFactory;
        private readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _next;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly RequestOptions _requestOptions;

        public MissingPipe(IDocumentClient client, string databaseName, string collectionName, IPipe<SagaConsumeContext<TSaga, TMessage>> next,
            IDocumentDbSagaConsumeContextFactory documentDbSagaConsumeContextFactory, JsonSerializerSettings jsonSerializerSettings)
        {
            _client = client;
            _databaseName = databaseName;
            _collectionName = collectionName;
            _next = next;
            _documentDbSagaConsumeContextFactory = documentDbSagaConsumeContextFactory;
            _jsonSerializerSettings = jsonSerializerSettings;
            if (_jsonSerializerSettings != null) _requestOptions = new RequestOptions { JsonSerializerSettings = jsonSerializerSettings };
        }

        public void Probe(ProbeContext context)
        {
            _next.Probe(context);
        }

        public async Task Send(SagaConsumeContext<TSaga, TMessage> context)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("SAGA:{0}:{1} Added {2}", TypeMetadataCache<TSaga>.ShortName, context.Saga.CorrelationId,
                    TypeMetadataCache<TMessage>.ShortName);

            SagaConsumeContext<TSaga, TMessage> proxy =
                _documentDbSagaConsumeContextFactory.Create(_client, _databaseName, _collectionName, context, context.Saga, false, _jsonSerializerSettings);

            await _next.Send(proxy).ConfigureAwait(false);

            if (!proxy.IsCompleted)
                await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), context.Saga, _requestOptions, true).ConfigureAwait(false);
        }
    }
}