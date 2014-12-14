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
    using MassTransit.PipeConfigurators;
    using MassTransit.Pipeline;
    using MassTransit.Pipeline.Configuration;
    using MassTransit.Policies;
    using MassTransit.Saga;
    using Saga.Pipeline;


    public class CorrelatedStateMachineSubscriptionConnector<TInstance, TMessage> :
        StateMachineSubscriptionConnector
        where TInstance : class, SagaStateMachineInstance
        where TMessage : class, CorrelatedBy<Guid>
    {
        readonly Event<TMessage> _dataEvent;
        readonly ISagaPolicy<TInstance, TMessage> _policy;
        readonly ISagaRepository<TInstance> _sagaRepository;
        readonly StateMachine<TInstance> _machine;

        public CorrelatedStateMachineSubscriptionConnector(StateMachine<TInstance> machine,
                                                           ISagaRepository<TInstance> sagaRepository,
                                                           Event<TMessage> dataEvent,
                                                           IEnumerable<State> states,
                                                           StateMachinePolicyFactory<TInstance> policyFactory,
                                                           Expression<Func<TInstance, bool>> removeExpression)
        {
            _machine = machine;
            _sagaRepository = sagaRepository;
            _dataEvent = dataEvent;

            _policy = policyFactory.GetPolicy<TMessage>(states, x => x.CorrelationId, removeExpression);
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
            return new CorrelatedStateMachineMessageSink<TInstance, TMessage>(_machine, _sagaRepository,
                _policy, _dataEvent);
        }

        public ConnectHandle Connect<TInstance1>(IConsumePipe consumePipe, ISagaRepository<TInstance1> sagaRepository, IRetryPolicy retryPolicy,
            params IPipeBuilderConfigurator<SagaConsumeContext<TInstance1>>[] pipeBuilderConfigurators) where TInstance1 : class, ISaga, SagaStateMachineInstance
        {
        }
    }
}