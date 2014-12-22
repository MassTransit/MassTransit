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
namespace Automatonymous.SubscriptionConnectors
{
    using MassTransit;
    using MassTransit.Pipeline;
    using MassTransit.Saga;
    using MassTransit.Saga.SubscriptionConnectors;


    public class StateMachineSagaMessageConnector<TInstance, TMessage> :
        SagaMessageConnector<TInstance, TMessage>
        where TInstance : class, ISaga, SagaStateMachineInstance
        where TMessage : class
    {
        readonly IFilter<ConsumeContext<TMessage>> _locatorFilter;
        readonly IFilter<SagaConsumeContext<TInstance, TMessage>> _messageFilter;

        public StateMachineSagaMessageConnector(IFilter<SagaConsumeContext<TInstance, TMessage>> messageFilter,
            IFilter<ConsumeContext<TMessage>> locatorFilter)
        {
            _messageFilter = messageFilter;
            _locatorFilter = locatorFilter;
        }

        protected override IFilter<SagaConsumeContext<TInstance, TMessage>> GetMessageFilter()
        {
            return _messageFilter;
        }

        protected override IFilter<ConsumeContext<TMessage>> GetLocatorFilter(ISagaRepository<TInstance> repository)
        {
            return _locatorFilter;
        }
    }
}