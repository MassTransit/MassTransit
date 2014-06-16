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
    using Magnum.Extensions;
    using Pipeline.Sinks;
    using Policies;
    using SubscriptionBuilders;


    public class InstanceSubscriptionConfiguratorImpl :
        SubscriptionConfiguratorImpl<InstanceSubscriptionConfigurator>,
        InstanceSubscriptionConfigurator,
        SubscriptionBuilderConfigurator
    {
        readonly object _instance;

        public InstanceSubscriptionConfiguratorImpl(object instance, IRetryPolicy retryPolicy)
            : base(retryPolicy)
        {
            _instance = instance;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_instance == null)
                yield return this.Failure("The instance cannot be null. This should have come in the ctor.");

            if (_instance != null && !_instance.GetType().Implements<IConsumer>())
            {
                yield return
                    this.Warning(string.Format("The instance of {0} does not implement any IMessageConsumer interfaces",
                        _instance.GetType().ToShortTypeName()));
            }
        }

        public SubscriptionBuilder Configure()
        {
            return new InstanceSubscriptionBuilder(_instance, RetryPolicy, ReferenceFactory);
        }
    }
}