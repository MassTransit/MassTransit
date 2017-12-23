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
namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System;
    using System.Threading.Tasks;

    using MassTransit.Context;
    using MassTransit.Logging;
    using MassTransit.Saga;
    using MassTransit.Util;

    using Microsoft.EntityFrameworkCore;


    public class EntityFrameworkSagaConsumeContext<TSaga, TMessage> :
        ConsumeContextProxyScope<TMessage>,
        SagaConsumeContext<TSaga, TMessage>
        where TMessage : class
        where TSaga : class, ISaga
    {
        static readonly ILog _log = Logger.Get<EntityFrameworkSagaRepository<TSaga>>();
        readonly DbContext _dbContext;
        readonly bool _existing;

        public EntityFrameworkSagaConsumeContext(DbContext dbContext, ConsumeContext<TMessage> context, TSaga instance, bool existing = true)
            : base(context)
        {
            this.Saga = instance;
            this._dbContext = dbContext;
            this._existing = existing;
        }

        Guid? MessageContext.CorrelationId => this.Saga.CorrelationId;

        SagaConsumeContext<TSaga, T> SagaConsumeContext<TSaga>.PopContext<T>()
        {
            var context = this as SagaConsumeContext<TSaga, T>;
            if (context == null)
                throw new ContextException($"The ConsumeContext<{TypeMetadataCache<TMessage>.ShortName}> could not be cast to {TypeMetadataCache<T>.ShortName}");

            return context;
        }

        public async Task SetCompleted()
        {
            this.IsCompleted = true;
            if (this._existing)
            {
                this._dbContext.Set<TSaga>().Remove(this.Saga);

                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("SAGA:{0}:{1} Removed {2}", TypeMetadataCache<TSaga>.ShortName, TypeMetadataCache<TMessage>.ShortName,
                        this.Saga.CorrelationId);
                }

                await this._dbContext.SaveChangesAsync(this.CancellationToken).ConfigureAwait(false);
            }
        }

        public bool IsCompleted { get; private set; }
        public TSaga Saga { get; }
    }
}