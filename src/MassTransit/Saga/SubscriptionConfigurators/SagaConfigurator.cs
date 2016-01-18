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
namespace MassTransit.Saga.SubscriptionConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configurators;
    using Connectors;
    using PipeConfigurators;


    public class SagaConfigurator<TSaga> :
        ISagaConfigurator<TSaga>,
        IReceiveEndpointSpecification
        where TSaga : class, ISaga
    {
        readonly IList<IPipeSpecification<SagaConsumeContext<TSaga>>> _pipeSpecifications;
        readonly ISagaRepository<TSaga> _sagaRepository;

        public SagaConfigurator(ISagaRepository<TSaga> sagaRepository)
        {
            _sagaRepository = sagaRepository;
            _pipeSpecifications = new List<IPipeSpecification<SagaConsumeContext<TSaga>>>();
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            SagaConnectorCache<TSaga>.Connector.ConnectSaga(builder, _sagaRepository, _pipeSpecifications.ToArray());
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_sagaRepository == null)
                yield return this.Failure("The saga repository cannot be null. How else are we going to save stuff? #facetopalm");

            foreach (ValidationResult result in _pipeSpecifications.SelectMany(x => x.Validate()))
                yield return result;
        }

        public void ConfigureMessage<T>(Action<ISagaMessageConfigurator<T>> configure)
            where T : class
        {
            var messageConfigurator = new SagaMessageConfigurator<TSaga, T>(this);

            configure(messageConfigurator);
        }

        public void AddPipeSpecification(IPipeSpecification<SagaConsumeContext<TSaga>> specification)
        {
            _pipeSpecifications.Add(specification);
        }
    }
}