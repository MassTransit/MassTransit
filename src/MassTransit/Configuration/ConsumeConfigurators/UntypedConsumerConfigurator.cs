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
    using ConsumeConnectors;
    using ConsumerSpecifications;
    using GreenPipes;
    using Internals.Extensions;
    using Metadata;
    using Pipeline.ConsumerFactories;
    using Util;


    public class UntypedConsumerConfigurator<TConsumer> :
        IConsumerConfigurator,
        IReceiveEndpointSpecification
        where TConsumer : class
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;
        readonly IConsumerSpecification<TConsumer> _specification;

        public UntypedConsumerConfigurator(Func<Type, object> consumerFactory, IConsumerConfigurationObserver observer)
        {
            _consumerFactory = new DelegateConsumerFactory<TConsumer>(() => (TConsumer)consumerFactory(typeof(TConsumer)));

            _specification = ConsumerConnectorCache<TConsumer>.Connector.CreateConsumerSpecification<TConsumer>();

            _specification.ConnectConsumerConfigurationObserver(observer);
        }

        public ConnectHandle ConnectConsumerConfigurationObserver(IConsumerConfigurationObserver observer)
        {
            return _specification.ConnectConsumerConfigurationObserver(observer);
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_consumerFactory == null)
                yield return this.Failure("The consumer factory cannot be null.");

            if (!typeof(TConsumer).HasInterface<IConsumer>())
            {
                yield return this.Warning($"The consumer class {TypeMetadataCache<TConsumer>.ShortName} does not implement any IMessageConsumer interfaces");
            }

            foreach (var result in _specification.Validate())
            {
                yield return result;
            }
        }

        public void Configure(IReceiveEndpointBuilder builder)
        {
            ConsumerConnectorCache<TConsumer>.Connector.ConnectConsumer(builder, _consumerFactory, _specification);
        }
    }
}