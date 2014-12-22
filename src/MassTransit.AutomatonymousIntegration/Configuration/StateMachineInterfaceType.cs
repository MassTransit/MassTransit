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
namespace Automatonymous
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using MassTransit;
    using MassTransit.Internals.Extensions;
    using MassTransit.Saga;
    using MassTransit.Saga.SubscriptionConnectors;
    using MassTransit.Util;
    using SubscriptionConnectors;


    public class StateMachineInterfaceType<TInstance, TData> :
        IStateMachineInterfaceType<TInstance>
        where TInstance : class, ISaga, SagaStateMachineInstance
        where TData : class
    {
        readonly Lazy<SagaConnectorFactory> _correlatedConnectorFactory;
        readonly Event<TData> _event;
        readonly Lazy<SagaConnectorFactory> _expressionConnectoryFactory;
        readonly StateMachine<TInstance> _machine;
        readonly StateMachinePolicyFactory<TInstance> _policyFactory;
        readonly StateMachineSagaRepository<TInstance> _repository;
        readonly IEnumerable<State<TInstance>> _states;

        public StateMachineInterfaceType(StateMachine<TInstance> machine, StateMachineSagaRepository<TInstance> repository,
            StateMachinePolicyFactory<TInstance> policyFactory, IEnumerable<State<TInstance>> states, Event<TData> @event)
        {
            _machine = machine;
            _repository = repository;
            _policyFactory = policyFactory;
            _states = states;
            _event = @event;

            _correlatedConnectorFactory = new Lazy<SagaConnectorFactory>(() => (SagaConnectorFactory)
                Activator.CreateInstance(
                    typeof(CorrelatedStateMachineEventConnectorFactory<,>).MakeGenericType(typeof(TInstance), typeof(TData)),
                    _machine, _repository, _policyFactory, _event, _states));

            _expressionConnectoryFactory = new Lazy<SagaConnectorFactory>(() => (SagaConnectorFactory)
                Activator.CreateInstance(
                    typeof(ExpressionStateMachineEventConnectorFactory<,>).MakeGenericType(typeof(TInstance), typeof(TData)),
                    _machine, _repository, _policyFactory, _event, _states));
        }

        public SagaMessageConnector GetConnector()
        {
            Expression<Func<TInstance, TData, bool>> expression;
            Func<TData, Guid> selector;
            if (_repository.TryGetCorrelationExpressionForEvent(_event, out expression, out selector))
                return _expressionConnectoryFactory.Value.CreateMessageConnector();

            if (typeof(TData).HasInterface(typeof(CorrelatedBy<>)))
                return _correlatedConnectorFactory.Value.CreateMessageConnector();

            throw new AutomatonymousException(typeof(TInstance), "No event connector found: " + TypeMetadataCache<TData>.ShortName);
        }
    }
}