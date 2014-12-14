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
    using Magnum.Extensions;
    using Magnum.Reflection;
    using MassTransit;


    public interface StateMachineEventConnectorFactory
    {
        IEnumerable<StateMachineSubscriptionConnector> Create();
    }


    public class StateMachineEventConnectorFactory<TInstance, TMessage> :
        StateMachineEventConnectorFactory
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class
    {
        readonly Event<TMessage> _event;
        readonly StateMachinePolicyFactory<TInstance> _policyFactory;
        readonly Expression<Func<TInstance, bool>> _removeExpression;
        readonly StateMachineSagaRepository<TInstance> _repository;
        readonly StateMachine<TInstance> _stateMachine;
        readonly IEnumerable<State> _states;

        public StateMachineEventConnectorFactory(StateMachine<TInstance> stateMachine,
                                                 StateMachineSagaRepository<TInstance> repository,
                                                 StateMachinePolicyFactory<TInstance> policyFactory,
                                                 Event<TMessage> @event, IEnumerable<State> states)
        {
            _stateMachine = stateMachine;
            _repository = repository;
            _policyFactory = policyFactory;
            _event = @event;
            _states = states;

            _removeExpression = repository.GetCompletedExpression();
        }

        public IEnumerable<StateMachineSubscriptionConnector> Create()
        {
            Expression<Func<TInstance, TMessage, bool>> expression;
            Func<TMessage, Guid> selector;
            if (_repository.TryGetCorrelationExpressionForEvent(_event, out expression, out selector))
            {
                yield return
                    (StateMachineSubscriptionConnector)
                    FastActivator.Create(typeof(ExpressionStateMachineSubscriptionConnector<,>),
                        new[] {typeof(TInstance), typeof(TMessage)},
                        new object[]
                            {
                                _stateMachine, _repository, _event, _states, _policyFactory,
                                _removeExpression, expression, selector
                            });
            }
            else if (typeof(TMessage).Implements<CorrelatedBy<Guid>>())
            {
                yield return
                    (StateMachineSubscriptionConnector)
                    FastActivator.Create(typeof(CorrelatedStateMachineSubscriptionConnector<,>),
                        new[] {typeof(TInstance), typeof(TMessage)},
                        new object[]
                            {
                                _stateMachine, _repository, _event, _states, _policyFactory,
                                _removeExpression
                            });
            }
            else
            {
                throw new NotSupportedException("No method to connect to event was found for "
                                                + typeof(TMessage).FullName);
            }
        }
    }
}