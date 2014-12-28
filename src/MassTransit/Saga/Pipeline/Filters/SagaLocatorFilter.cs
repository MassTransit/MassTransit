// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Saga.Pipeline.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Context;
    using MassTransit.Pipeline;
    using Util;


    /// <summary>
    ///     Delivers a message through the saga repository to the saga instance
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class SagaLocatorFilter<TSaga, TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly ISagaLocator<TMessage> _locator;
        readonly ISagaPolicy<TSaga, TMessage> _policy;

        public SagaLocatorFilter(ISagaLocator<TMessage> locator, ISagaPolicy<TSaga, TMessage> policy)
        {
            _locator = locator;
            _policy = policy;
        }

        async Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            Stopwatch timer = Stopwatch.StartNew();
            try
            {
                IEnumerable<Guid> sagaIds = await _locator.Find(context);

                foreach (Guid sagaId in sagaIds)
                {
                    // create a nested scope for each saga instance
                    var consumeContext = new ConsumeContextScope<TMessage>(context);

                    SagaContext<TSaga, TMessage> sagaContext = new SagaContextImpl<TSaga, TMessage>(sagaId, _policy);

                    consumeContext.GetOrAddPayload(() => sagaContext);

                    await next.Send(consumeContext);
                }
            }
            catch (Exception ex)
            {
                context.NotifyFaulted(TypeMetadataCache<TSaga>.ShortName, ex);
                throw;
            }
        }

        bool IFilter<ConsumeContext<TMessage>>.Inspect(IPipeInspector inspector)
        {
            return inspector.Inspect(this);
        }
    }
}