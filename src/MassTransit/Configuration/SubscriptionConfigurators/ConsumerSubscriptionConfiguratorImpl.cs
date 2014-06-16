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
namespace MassTransit.SubscriptionConfigurators
{
    using System.Collections.Generic;
    using Configurators;
    using Pipeline.Sinks;
    using Policies;
    using SubscriptionBuilders;


    public class ConsumerSubscriptionConfiguratorImpl<TConsumer> :
        SubscriptionConfiguratorImpl<ConsumerSubscriptionConfigurator<TConsumer>>,
        ConsumerSubscriptionConfigurator<TConsumer>,
        SubscriptionBuilderConfigurator
        where TConsumer : class
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;

        public ConsumerSubscriptionConfiguratorImpl(IConsumerFactory<TConsumer> consumerFactory,
            IRetryPolicy retryPolicy)
            : base(retryPolicy)
        {
            _consumerFactory = consumerFactory;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            return _consumerFactory.Validate();
        }

        public SubscriptionBuilder Configure()
        {
            return new ConsumerSubscriptionBuilder<TConsumer>(_consumerFactory, RetryPolicy, ReferenceFactory);
        }
    }
}