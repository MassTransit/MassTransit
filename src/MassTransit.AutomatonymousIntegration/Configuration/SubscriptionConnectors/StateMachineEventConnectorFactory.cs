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
namespace Automatonymous.SubscriptionConnectors
{
    using MassTransit.Saga;
    using MassTransit.Saga.Connectors;
    using MassTransit.Saga.Pipeline.Filters;
    using Pipeline;


    public class StateMachineEventConnectorFactory<TInstance, TMessage> :
        SagaConnectorFactory
        where TInstance : class, ISaga, SagaStateMachineInstance
        where TMessage : class
    {
        readonly StateMachineSagaMessageFilter<TInstance, TMessage> _consumeFilter;
        readonly SagaLocatorFilter<TInstance, TMessage> _locatorFilter;

        public StateMachineEventConnectorFactory(SagaStateMachine<TInstance> stateMachine, EventCorrelation<TInstance, TMessage> correlation,
            ISagaLocator<TMessage> locator)
        {
            ISagaPolicy<TInstance, TMessage> policy = correlation.Policy;

            _consumeFilter = new StateMachineSagaMessageFilter<TInstance, TMessage>(stateMachine, correlation.Event);

            _locatorFilter = new SagaLocatorFilter<TInstance, TMessage>(locator, policy);
        }

        public SagaMessageConnector CreateMessageConnector()
        {
            return new StateMachineSagaMessageConnector<TInstance, TMessage>(_consumeFilter, _locatorFilter);
        }
    }
}