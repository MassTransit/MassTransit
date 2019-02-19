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
namespace MassTransit.ConsumeConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConsumeConnectors;
    using ConsumerSpecifications;
    using GreenPipes;


    public class ConsumerConfigurator<TConsumer> :
        IConsumerConfigurator<TConsumer>,
        IReceiveEndpointSpecification
        where TConsumer : class, IConsumer
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;
        readonly IConsumerSpecification<TConsumer> _specification;

        public ConsumerConfigurator(IConsumerFactory<TConsumer> consumerFactory, IConsumerConfigurationObserver observer)
        {
            _consumerFactory = consumerFactory;

            _specification = ConsumerConnectorCache<TConsumer>.Connector.CreateConsumerSpecification<TConsumer>();

            _specification.ConnectConsumerConfigurationObserver(observer);
        }

        public void AddPipeSpecification(IPipeSpecification<ConsumerConsumeContext<TConsumer>> specification)
        {
            _specification.AddPipeSpecification(specification);
        }

        public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _specification.ConnectConsumerConfigurationObserver(observer);
        }

        void IConsumerConfigurator<TConsumer>.Message<T>(Action<IConsumerMessageConfigurator<T>> configure)
        {
            _specification.Message(configure);
        }

        void IConsumerConfigurator<TConsumer>.ConsumerMessage<T>(Action<IConsumerMessageConfigurator<TConsumer, T>> configure)
        {
            _specification.ConsumerMessage(configure);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _consumerFactory.Validate().Concat(_specification.Validate());
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            ConsumerConnectorCache<TConsumer>.Connector.ConnectConsumer(builder, _consumerFactory, _specification);
        }
    }
}