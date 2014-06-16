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
namespace MassTransit.SubscriptionBuilders
{
    using Pipeline;
    using Pipeline.Sinks;
    using Policies;
    using Subscriptions;


    public class HandlerSubscriptionBuilder<TMessage> :
        SubscriptionBuilder
        where TMessage : class
    {
        readonly MessageHandler<TMessage> _handler;
        readonly ReferenceFactory _referenceFactory;
        readonly IRetryPolicy _retryPolicy;

        public HandlerSubscriptionBuilder(MessageHandler<TMessage> handler, IRetryPolicy retryPolicy,
            ReferenceFactory referenceFactory)
        {
            _handler = handler;
            _referenceFactory = referenceFactory;
            _retryPolicy = retryPolicy;
        }

        public ISubscriptionReference Subscribe(IInboundPipe filter)
        {
            ConnectHandle handle = filter.ConnectHandler(_handler, _retryPolicy);

            return _referenceFactory(handle);
        }
    }
}