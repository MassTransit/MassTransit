// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Configurators;
    using MassTransit.PipeConfigurators;
    using MassTransit.Saga;
    using MassTransit.Saga.SubscriptionConfigurators;
    using SubscriptionConnectors;


    public class StateMachineSagaSpecification<TInstance> :
        ISagaConfigurator<TInstance>,
        IReceiveEndpointSpecification
        where TInstance : class, SagaStateMachineInstance
    {
        readonly IList<IPipeSpecification<SagaConsumeContext<TInstance>>> _pipeSpecifications;
        readonly ISagaRepository<TInstance> _repository;
        readonly SagaStateMachine<TInstance> _stateMachine;

        public StateMachineSagaSpecification(SagaStateMachine<TInstance> stateMachine,
            ISagaRepository<TInstance> repository)
        {
            _stateMachine = stateMachine;
            _repository = repository;
            _pipeSpecifications = new List<IPipeSpecification<SagaConsumeContext<TInstance>>>();
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_stateMachine == null)
                yield return this.Failure("StateMachine", "must not be null");
            if (_repository == null)
                yield return this.Failure("Repository", "must not be null");
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            var connector = new StateMachineConnector<TInstance>(_stateMachine);

            connector.ConnectSaga(builder, _repository, _pipeSpecifications.ToArray());
        }

        public void ConfigureMessage<T>(Action<ISagaMessageConfigurator<T>> configure)
            where T : class
        {
            var messageConfigurator = new SagaMessageConfigurator<TInstance, T>(this);

            configure(messageConfigurator);
        }

        public void AddPipeSpecification(IPipeSpecification<SagaConsumeContext<TInstance>> specification)
        {
            _pipeSpecifications.Add(specification);
        }
    }
}