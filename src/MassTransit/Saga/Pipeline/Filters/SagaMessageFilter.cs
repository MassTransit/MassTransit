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
    using System.Diagnostics;
    using System.Threading.Tasks;
    using MassTransit.Pipeline;
    using Util;


    /// <summary>
    /// Delivers a message through the saga repository to the saga instance
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class SagaMessageFilter<TSaga, TMessage> :
        IFilter<ConsumeContext<TMessage>>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _sagaPipe;
        readonly ISagaRepository<TSaga> _sagaRepository;

        public SagaMessageFilter(ISagaRepository<TSaga> sagaRepository, IPipe<SagaConsumeContext<TSaga, TMessage>> sagaPipe)
        {
            _sagaRepository = sagaRepository;
            _sagaPipe = sagaPipe;
        }

        async Task IFilter<ConsumeContext<TMessage>>.Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
        {
            Stopwatch timer = Stopwatch.StartNew();
            try
            {
                await _sagaRepository.Send(context, _sagaPipe).ConfigureAwait(false);

                context.NotifyConsumed(timer.Elapsed, TypeMetadataCache<TSaga>.ShortName);

                await next.Send(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                context.NotifyFaulted(timer.Elapsed, TypeMetadataCache<TSaga>.ShortName, ex);
                throw;
            }
        }

        bool IFilter<ConsumeContext<TMessage>>.Visit(IPipelineVisitor visitor)
        {
            return visitor.Visit(this, x => _sagaPipe.Visit(x));
        }
    }
}