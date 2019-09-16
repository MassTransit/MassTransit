// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.RedisIntegration
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using Metadata;
    using Util;


    public class RedisSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextProxyScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, IVersionedSaga
    {
        readonly ITypedDatabase<TSaga> _sagas;

        public RedisSagaConsumeContext(ITypedDatabase<TSaga> sagas, ConsumeContext<TMessage> context, TSaga instance)
            : base(context)
        {
            _sagas = sagas;

            Saga = instance;
        }

        Guid? MessageContext.CorrelationId => Saga.CorrelationId;

        async Task SagaConsumeContext<TSaga>.SetCompleted()
        {
            await _sagas.Delete(Saga.CorrelationId).ConfigureAwait(false);

            IsCompleted = true;

            LogContext.Debug?.Log("SAGA:{SagaType}:{CorrelationId} Removed {MessageType}", TypeMetadataCache<TSaga>.ShortName, Saga.CorrelationId,
                TypeMetadataCache<TMessage>.ShortName);
        }

        public TSaga Saga { get; }
        public bool IsCompleted { get; private set; }
    }
}
