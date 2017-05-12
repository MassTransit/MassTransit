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
namespace MassTransit.DocumentDbIntegration.Saga.Context
{
    using Logging;
    using MassTransit.Context;
    using MassTransit.Saga;
    using Microsoft.Azure.Documents.Client;
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Util;

    public class DocumentDbSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextProxyScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        static readonly ILog _log = Logger.Get<DocumentDbSagaRepository<TSaga>>();
        readonly IDocumentClient _client;
        readonly bool _existing;
        readonly string _databaseName;
        readonly string _collectionName;

        public DocumentDbSagaConsumeContext(IDocumentClient client, string databaseName, string collectionName, ConsumeContext<TMessage> context, TSaga instance, bool existing = true)
            : base(context)
        {
            Saga = instance;
            _client = client;
            _existing = existing;
            _databaseName = databaseName;
            _collectionName = collectionName;
        }

        Guid? MessageContext.CorrelationId => Saga.CorrelationId;

        public SagaConsumeContext<TSaga, T> PopContext<T>() where T : class
        {
            var context = this as SagaConsumeContext<TSaga, T>;
            if (context == null)
                throw new ContextException($"The ConsumeContext<{TypeMetadataCache<TMessage>.ShortName}> could not be cast to {TypeMetadataCache<T>.ShortName}");

            return context;
        }

        public async Task SetCompleted()
        {
            IsCompleted = true;

            if (_existing)
            {
                await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseName, _collectionName, Saga.CorrelationId.ToString())).ConfigureAwait(false);

                if (_log.IsDebugEnabled)
                    _log.DebugFormat("SAGA:{0}:{1} Removed {2}", TypeMetadataCache<TSaga>.ShortName, TypeMetadataCache<TMessage>.ShortName, Saga.CorrelationId);
            }
        }

        public TSaga Saga { get; }

        public bool IsCompleted { get; private set; }
    }
}