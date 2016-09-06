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
namespace MassTransit.Saga.Connectors
{
    using System;
    using GreenPipes;
    using MassTransit.Pipeline;
    using Pipeline.Filters;


    /// <summary>
    /// Connects a message that has an exact CorrelationId to the saga instance
    /// to the saga repository.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public class CorrelatedSagaMessageConnector<TSaga, TMessage> :
        SagaMessageConnector<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly IFilter<SagaConsumeContext<TSaga, TMessage>> _consumeFilter;
        readonly Func<ConsumeContext<TMessage>, Guid> _correlationIdSelector;
        readonly ISagaPolicy<TSaga, TMessage> _policy;

        public CorrelatedSagaMessageConnector(IFilter<SagaConsumeContext<TSaga, TMessage>> consumeFilter, ISagaPolicy<TSaga, TMessage> policy,
            Func<ConsumeContext<TMessage>, Guid> correlationIdSelector)
        {
            _consumeFilter = consumeFilter;
            _policy = policy;
            _correlationIdSelector = correlationIdSelector;
        }

        protected override void ConfigureSagaPipe(IPipeConfigurator<SagaConsumeContext<TSaga, TMessage>> configurator)
        {
            configurator.UseFilter(_consumeFilter);
        }

        protected override void ConfigureMessagePipe(IPipeConfigurator<ConsumeContext<TMessage>> configurator, ISagaRepository<TSaga> repository,
            IPipe<SagaConsumeContext<TSaga, TMessage>> sagaPipe)
        {
            configurator.UseFilter(new CorrelationIdMessageFilter<TMessage>(_correlationIdSelector));
            configurator.UseFilter(new CorrelatedSagaFilter<TSaga, TMessage>(repository, _policy, sagaPipe));
        }
    }
}