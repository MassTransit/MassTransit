// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using MassTransit.Pipeline;
    using Monitoring.Introspection;
    using Util;


    /// <summary>
    /// Dispatches the ConsumeContext to the consumer method for the specified message type
    /// </summary>
    /// <typeparam name="TSaga">The consumer type</typeparam>
    /// <typeparam name="TMessage">The message type</typeparam>
    public class InitiatedBySagaMessageFilter<TSaga, TMessage> :
        ISagaMessageFilter<TSaga, TMessage>
        where TSaga : class, ISaga, InitiatedBy<TMessage>
        where TMessage : class, CorrelatedBy<Guid>
    {
        async Task IProbeSite.Probe(ProbeContext context)
        {
        }

        public async Task Send(SagaConsumeContext<TSaga, TMessage> context, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
        {
            var consumer = context.Saga as InitiatedBy<TMessage>;
            if (consumer == null)
            {
                string message = string.Format("Saga type {0} is not initiated by message type {1}",
                    TypeMetadataCache<TSaga>.ShortName, TypeMetadataCache<TMessage>.ShortName);

                throw new ConsumerMessageException(message);
            }

            await consumer.Consume(context);

            await next.Send(context);
        }

        public bool Visit(IPipelineVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}