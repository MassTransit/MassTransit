// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Distributor.WorkerConfigurators
{
    using System;
    using System.Collections.Generic;
    using Builders;
    using Configuration;
    using Configurators;
    using MassTransit.Configuration;
    using SubscriptionConfigurators;
    using WorkerConnectors;

    public class UntypedConsumerWorkerConfigurator<TConsumer> :
        SubscriptionConfiguratorImpl<ConsumerWorkerConfigurator>,
        ConsumerWorkerConfigurator,
        WorkerBuilderConfigurator
        where TConsumer : class
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;

        public UntypedConsumerWorkerConfigurator(Func<Type, object> consumerFactory)
        {
            _consumerFactory =
                new DelegateConsumerFactory<TConsumer>(() => (TConsumer)consumerFactory(typeof(TConsumer)));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _consumerFactory.Validate();
        }

        public void Configure(WorkerBuilder builder)
        {
            var configurator = new ConsumerWorkerConnector<TConsumer>(ReferenceFactory, _consumerFactory);

            builder.Add(configurator);
        }
    }
}