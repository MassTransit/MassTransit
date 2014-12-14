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
namespace Automatonymous.SubscriptionBuilders
{
    using MassTransit;
    using MassTransit.Pipeline;
    using MassTransit.SubscriptionBuilders;
    using MassTransit.Subscriptions;
    using SubscriptionConnectors;


    public class StateMachineSubscriptionBuilder<TInstance> :
        SubscriptionBuilder
        where TInstance : class, SagaStateMachineInstance
    {
        readonly StateMachineConnector<TInstance> _connector;
        readonly ReferenceFactory _referenceFactory;

        public StateMachineSubscriptionBuilder(StateMachine<TInstance> stateMachine,
            StateMachineSagaRepository<TInstance> repository,
            ReferenceFactory referenceFactory)
        {
            _connector = new StateMachineConnector<TInstance>(stateMachine, repository);
            _referenceFactory = referenceFactory;
        }

        public ISubscriptionReference Subscribe(IInboundPipelineConfigurator configurator)
        {
            UnsubscribeAction unsubscribe = _connector.Connect(configurator);

            return _referenceFactory(unsubscribe);
        }
    }
}