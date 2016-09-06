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
    using CorrelationConfigurators;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Pipeline;
    using MassTransit.Saga;
    using MassTransit.Saga.ConnectorFactories;
    using MassTransit.Saga.Connectors;
    using Pipeline;


    public class StateMachineEventConnectorFactory<TInstance, TMessage> :
        ISagaConnectorFactory
        where TInstance : class, ISaga, SagaStateMachineInstance
        where TMessage : class
    {
        readonly StateMachineSagaMessageFilter<TInstance, TMessage> _consumeFilter;
        readonly IFilter<ConsumeContext<TMessage>> _messageFilter;
        readonly ISagaPolicy<TInstance, TMessage> _policy;
        readonly SagaFilterFactory<TInstance, TMessage> _sagaFilterFactory;

        public StateMachineEventConnectorFactory(SagaStateMachine<TInstance> stateMachine, EventCorrelation<TInstance, TMessage> correlation)
        {
            _consumeFilter = new StateMachineSagaMessageFilter<TInstance, TMessage>(stateMachine, correlation.Event);

            _sagaFilterFactory = correlation.FilterFactory;
            _policy = correlation.Policy;
            _messageFilter = correlation.MessageFilter;
        }

        public ISagaMessageConnector CreateMessageConnector()
        {
            return new StateMachineSagaMessageConnector<TInstance, TMessage>(_consumeFilter, _policy, _sagaFilterFactory, _messageFilter);
        }
    }
}