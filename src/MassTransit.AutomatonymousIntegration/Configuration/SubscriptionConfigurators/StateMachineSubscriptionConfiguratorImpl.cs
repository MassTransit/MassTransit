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
namespace Automatonymous.SubscriptionConfigurators
{
    using System.Collections.Generic;
    using MassTransit.Configurators;
    using MassTransit.EndpointConfigurators;
    using MassTransit.SubscriptionConfigurators;


    public class StateMachineSubscriptionConfiguratorImpl<TInstance> :
        SubscriptionConfiguratorImpl<StateMachineSubscriptionConfigurator<TInstance>>,
        StateMachineSubscriptionConfigurator<TInstance>,
        IReceiveEndpointBuilderConfigurator
        where TInstance : class, SagaStateMachineInstance
    {
        readonly StateMachineSagaRepository<TInstance> _repository;
        readonly StateMachine<TInstance> _stateMachine;

        public StateMachineSubscriptionConfiguratorImpl(StateMachine<TInstance> stateMachine,
            StateMachineSagaRepository<TInstance> repository)
        {
            _stateMachine = stateMachine;
            _repository = repository;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_stateMachine == null)
                yield return this.Failure("The state machine cannot be null.");
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
        }

//        public SubscriptionBuilder Configure()
//        {
//            return new StateMachineSubscriptionBuilder<TInstance>(_stateMachine, _repository, ReferenceFactory);
//        }
    }
}