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
namespace MassTransit.SagaConfigurators
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using Saga;
    using Saga.Connectors;


    public class SagaConfigurator<TSaga> :
        ISagaConfigurator<TSaga>,
        IReceiveEndpointSpecification
        where TSaga : class, ISaga
    {
        readonly ISagaRepository<TSaga> _sagaRepository;
        readonly ISagaSpecification<TSaga> _specification;

        public SagaConfigurator(ISagaRepository<TSaga> sagaRepository, ISagaConfigurationObserver observer)
        {
            _sagaRepository = sagaRepository;

            _specification = SagaConnectorCache<TSaga>.Connector.CreateSagaSpecification<TSaga>();

            _specification.ConnectSagaConfigurationObserver(observer);
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            SagaConnectorCache<TSaga>.Connector.ConnectSaga(builder, _sagaRepository, _specification);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_sagaRepository == null)
                yield return this.Failure("The saga repository cannot be null. How else are we going to save stuff? #facetopalm");

            foreach (var result in _specification.Validate())
                yield return result;
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

        public void SagaMessage<T>(Action<ISagaMessageConfigurator<TSaga, T>> configure)
            where T : class
        {
            _specification.SagaMessage(configure);
        }

        public void AddPipeSpecification(IPipeSpecification<SagaConsumeContext<TSaga>> specification)
        {
            _specification.AddPipeSpecification(specification);
        }

        public ConnectHandle ConnectSagaConfigurationObserver(ISagaConfigurationObserver observer)
        {
            return _specification.ConnectSagaConfigurationObserver(observer);
        }
    }
}
