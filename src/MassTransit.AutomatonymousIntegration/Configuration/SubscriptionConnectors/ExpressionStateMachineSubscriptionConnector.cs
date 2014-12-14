// Copyright 2011 Chris Patterson, Dru Sellers
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
    using MassTransit;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Configuration;
    using MassTransit.Saga;
    using Saga.Pipeline;


    public class ExpressionStateMachineSubscriptionConnector<TInstance, TMessage> :
        StateMachineSubscriptionConnector
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class
    {
        readonly Event<TMessage> _dataEvent;
        readonly ISagaPolicy<TInstance, TMessage> _policy;
        readonly ISagaRepository<TInstance> _sagaRepository;
        readonly Expression<Func<TInstance, TMessage, bool>> _selector;
        readonly StateMachine<TInstance> _stateMachine;

        public ExpressionStateMachineSubscriptionConnector(StateMachine<TInstance> stateMachine,
            ISagaRepository<TInstance> sagaRepository,
            Event<TMessage> dataEvent,
            IEnumerable<State> states,
            StateMachinePolicyFactory<TInstance> policyFactory,
            Expression<Func<TInstance, bool>> removeExpression,
            Expression<Func<TInstance, TMessage, bool>> selector,
            Func<TMessage, Guid> correlationIdSelector)
        {
            _stateMachine = stateMachine;
            _sagaRepository = sagaRepository;
            _dataEvent = dataEvent;
            _selector = selector;

            _policy = policyFactory.GetPolicy(states, correlationIdSelector, removeExpression);
        }

        public Type MessageType
        {
            get { return typeof(TMessage); }
        }

        public UnsubscribeAction Connect(IInboundPipelineConfigurator configurator)
        {
            ISagaMessageSink<TInstance, TMessage> sink = CreateSink();

            return configurator.Pipeline.ConnectToRouter(sink, () => configurator.SubscribedTo<TMessage>());
        }

        ISagaMessageSink<TInstance, TMessage> CreateSink()
        {
            return new ExpressionStateMachineMessageSink<TInstance, TMessage>(_stateMachine, _sagaRepository,
                _policy, _dataEvent, _selector);
        }
    }
}