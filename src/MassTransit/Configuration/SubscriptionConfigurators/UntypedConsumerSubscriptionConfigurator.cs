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
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Configurators;
    using Internals.Extensions;
    using Magnum.Extensions;
    using Policies;
    using SubscriptionBuilders;
    using Util;


    public class UntypedConsumerSubscriptionConfigurator<TConsumer> :
        SubscriptionConfiguratorImpl<ConsumerSubscriptionConfigurator>,
        ConsumerSubscriptionConfigurator,
        SubscriptionBuilderConfigurator
        where TConsumer : class
    {
        readonly IConsumerFactory<TConsumer> _consumerFactory;

        public UntypedConsumerSubscriptionConfigurator(Func<Type, object> consumerFactory, IRetryPolicy retryPolicy)
            : base(retryPolicy)
        {
            _consumerFactory =
                new DelegateConsumerFactory<TConsumer>(() => (TConsumer)consumerFactory(typeof(TConsumer)));
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_consumerFactory == null)
                yield return this.Failure("The consumer factory cannot be null.");

            if (!typeof(TConsumer).Implements<IConsumer>())
            {
                yield return
                    this.Warning(
                        string.Format("The consumer class {0} does not implement any IMessageConsumer interfaces",
                            TypeMetadataCache<TConsumer>.ShortName));
            }
        }

        public SubscriptionBuilder Configure()
        {
            return new ConsumerSubscriptionBuilder<TConsumer>(_consumerFactory, RetryPolicy, ReferenceFactory);
        }
    }
}