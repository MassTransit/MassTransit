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
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using MassTransit.Saga;
    using MassTransit.Saga.Pipeline.Filters;
    using MassTransit.Saga.SubscriptionConnectors;
    using MassTransit.Util;
    using Saga.Pipeline;


    public class ExpressionStateMachineEventConnectorFactory<TInstance, TMessage> :
        SagaConnectorFactory
        where TInstance : class, ISaga, SagaStateMachineInstance
        where TMessage : class
    {
        readonly StateMachineSagaMessageFilter<TInstance, TMessage> _consumeFilter;
        readonly SagaLocatorFilter<TInstance, TMessage> _locatorFilter;

        public ExpressionStateMachineEventConnectorFactory(StateMachine<TInstance> stateMachine,
            StateMachineSagaRepository<TInstance> repository,
            StateMachinePolicyFactory<TInstance> policyFactory,
            Event<TMessage> @event, IEnumerable<State> states)
        {
            Expression<Func<TInstance, TMessage, bool>> filterExpression;
            Func<TMessage, Guid> correlationIdSelector;
            if (!repository.TryGetCorrelationExpressionForEvent(@event, out filterExpression, out correlationIdSelector))
                throw new ArgumentException("The correlation expression was not found: " + TypeMetadataCache<TMessage>.ShortName);

            Expression<Func<TInstance, bool>> removeExpression = repository.GetCompletedExpression();

            ISagaPolicy<TInstance, TMessage> policy = policyFactory.GetPolicy(states, correlationIdSelector, removeExpression);

            _consumeFilter = new StateMachineSagaMessageFilter<TInstance, TMessage>(stateMachine, @event);

            var locator = new PropertySagaLocator<TInstance, TMessage>(repository, policy, filterExpression);

            _locatorFilter = new SagaLocatorFilter<TInstance, TMessage>(locator, policy);
        }

        public SagaMessageConnector CreateMessageConnector()
        {
            return new StateMachineSagaMessageConnector<TInstance, TMessage>(_consumeFilter, _locatorFilter);
        }
    }
}