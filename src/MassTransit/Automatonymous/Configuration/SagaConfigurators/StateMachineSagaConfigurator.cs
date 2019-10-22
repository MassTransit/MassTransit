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
namespace Automatonymous.SagaConfigurators
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Saga;
    using MassTransit.SagaConfigurators;
    using StateMachineConnectors;


    public class StateMachineSagaConfigurator<TInstance> :
        ISagaConfigurator<TInstance>,
        IReceiveEndpointSpecification
        where TInstance : class, SagaStateMachineInstance
    {
        readonly StateMachineConnector<TInstance> _connector;
        readonly ISagaRepository<TInstance> _repository;
        readonly ISagaSpecification<TInstance> _specification;
        readonly SagaStateMachine<TInstance> _stateMachine;

        public StateMachineSagaConfigurator(SagaStateMachine<TInstance> stateMachine, ISagaRepository<TInstance> repository,
            ISagaConfigurationObserver observer)
        {
            _stateMachine = stateMachine;
            _repository = repository;

            _connector = new StateMachineConnector<TInstance>(_stateMachine);

            _specification = _connector.CreateSagaSpecification<TInstance>();

            _specification.ConnectSagaConfigurationObserver(observer);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_stateMachine == null)
                yield return this.Failure("StateMachine", "must not be null");

            if (_repository == null)
                yield return this.Failure("Repository", "must not be null");

            foreach (var result in _specification.Validate())
                yield return result;
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            _connector.ConnectSaga(builder, _repository, _specification);
        }

        public void ConfigureMessage<T>(Action<ISagaMessageConfigurator<T>> configure)
            where T : class
        {
            _specification.Message(configure);
        }

        public void Message<T>(Action<ISagaMessageConfigurator<T>> configure)
            where T : class
        {
            _specification.Message(configure);
        }

        public void SagaMessage<T>(Action<ISagaMessageConfigurator<TInstance, T>> configure)
            where T : class
        {
            _specification.SagaMessage(configure);
        }

        public void AddPipeSpecification(IPipeSpecification<SagaConsumeContext<TInstance>> specification)
        {
            _specification.AddPipeSpecification(specification);
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _specification.ConnectSagaConfigurationObserver(observer);
        }
    }
}
