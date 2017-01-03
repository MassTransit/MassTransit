// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using MassTransit.Saga;
    using MassTransit.Saga.ConnectorFactories;
    using MassTransit.Saga.Connectors;
    using Pipeline;


    public class StateMachineEventConnectorFactory<TInstance, TMessage> :
        ISagaConnectorFactory
        where TInstance : class, ISaga, SagaStateMachineInstance
        where TMessage : class
    {
        readonly ISagaMessageConnector<TInstance> _connector;

        public StateMachineEventConnectorFactory(SagaStateMachine<TInstance> stateMachine, EventCorrelation<TInstance, TMessage> correlation)
        {
            var consumeFilter = new StateMachineSagaMessageFilter<TInstance, TMessage>(stateMachine, correlation.Event);

            _connector = new StateMachineSagaMessageConnector<TInstance, TMessage>(consumeFilter, correlation.Policy, correlation.FilterFactory,
                correlation.MessageFilter);
        }

        ISagaMessageConnector<T> ISagaConnectorFactory.CreateMessageConnector<T>()
        {
            var connector = _connector as ISagaMessageConnector<T>;
            if (connector == null)
                throw new ArgumentException("The saga type did not match the connector type");

            return connector;
        }
    }
}