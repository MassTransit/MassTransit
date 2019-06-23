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
namespace MassTransit.EntityFrameworkIntegration.Saga
{
    using System;
    using System.Data.Entity;
    using System.Threading.Tasks;
    using Context;
    using MassTransit.Saga;
    using Util;

    public class EntityFrameworkSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextProxyScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        readonly DbContext _dbContext;
        readonly bool _existing;

        public EntityFrameworkSagaConsumeContext(DbContext dbContext, ConsumeContext<TMessage> context, TSaga instance, bool existing = true)
            : base(context)
        {
            Saga = instance;
            _dbContext = dbContext;
            _existing = existing;
        }

        Guid? MessageContext.CorrelationId => Saga.CorrelationId;

        public async Task SetCompleted()
        {
            IsCompleted = true;
            if (_existing)
            {
                _dbContext.Set<TSaga>().Remove(Saga);

                LogContext.Debug?.Log("SAGA:{SagaType}:{CorrelationId} Removed {MessageType}", TypeMetadataCache<TSaga>.ShortName, Saga.CorrelationId,
                    TypeMetadataCache<TMessage>.ShortName);

                await _dbContext.SaveChangesAsync(CancellationToken).ConfigureAwait(false);
            }
        }

        public bool IsCompleted { get; private set; }
        public TSaga Saga { get; }
    }
}
