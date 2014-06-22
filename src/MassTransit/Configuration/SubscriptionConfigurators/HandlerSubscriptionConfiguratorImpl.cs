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
    using Configurators;
    using Policies;
    using SubscriptionBuilders;


    public class HandlerSubscriptionConfiguratorImpl<TMessage> :
        SubscriptionConfiguratorImpl<HandlerSubscriptionConfigurator<TMessage>>,
        HandlerSubscriptionConfigurator<TMessage>,
        SubscriptionBuilderConfigurator
        where TMessage : class
    {
        MessageHandler<TMessage> _handler;

        public HandlerSubscriptionConfiguratorImpl(MessageHandler<TMessage> handler,
            IRetryPolicy retryPolicy)
            : base(retryPolicy)
        {
            _handler = handler;
        }

        public HandlerSubscriptionConfigurator<TMessage> Where(Func<ConsumeContext<TMessage>, bool> filter)
        {
            MessageHandler<TMessage> handler = _handler;

            _handler = async context =>
            {
                if (filter(context))
                    await handler(context);
            };

            return this;
        }

        public IEnumerable<ValidationResult> Validate()
        {
            if (_handler == null)
                yield return this.Failure("The handler cannot be null. This should have come from the ctor.");
        }

        public SubscriptionBuilder Configure()
        {
            return new HandlerSubscriptionBuilder<TMessage>(_handler, RetryPolicy, ReferenceFactory);
        }
    }
}