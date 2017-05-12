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
    using MassTransit.Saga;
    using Microsoft.Azure.Documents.Client;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Util;

    public class MissingPipe<TSaga, TMessage> :
        IPipe<SagaConsumeContext<TSaga, TMessage>>
        where TSaga : class, ISaga
        where TMessage : class
    {
        static readonly ILog _log = Logger.Get<DocumentDbSagaRepository<TSaga>>();
        readonly string _databaseName;
        readonly string _collectionName;
        readonly IDocumentClient _client;
        readonly IDocumentDbSagaConsumeContextFactory _documentDbSagaConsumeContextFactory;
        readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _next;

        public MissingPipe(IDocumentClient client, string databaseName, string collectionName, IPipe<SagaConsumeContext<TSaga, TMessage>> next,
            IDocumentDbSagaConsumeContextFactory documentDbSagaConsumeContextFactory)
        {
            _client = client;
            _databaseName = databaseName;
            _collectionName = collectionName;
            _next = next;
            _documentDbSagaConsumeContextFactory = documentDbSagaConsumeContextFactory;
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

            SagaConsumeContext<TSaga, TMessage> proxy = _documentDbSagaConsumeContextFactory.Create(_client, _databaseName, _collectionName, context, context.Saga, false);

            await _next.Send(proxy).ConfigureAwait(false);

            if (!proxy.IsCompleted)
                await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), context.Saga, disableAutomaticIdGeneration: true).ConfigureAwait(false);
        }
    }
}