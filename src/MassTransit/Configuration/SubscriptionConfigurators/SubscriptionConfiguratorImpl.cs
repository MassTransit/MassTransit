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
    using Policies;
    using Subscriptions;


    public class SubscriptionConfiguratorImpl<TInterface> :
        SubscriptionConfigurator<TInterface>
        where TInterface : class, SubscriptionConfigurator<TInterface>
    {
        ReferenceFactory _referenceFactory;
        IRetryPolicy _retryPolicy;

        protected SubscriptionConfiguratorImpl()
            : this(Retry.None)
        {
        }

        protected SubscriptionConfiguratorImpl(IRetryPolicy retryPolicy)
        {
            Permanent();
            _retryPolicy = Retry.None;
        }

        protected ReferenceFactory ReferenceFactory
        {
            get { return _referenceFactory; }
        }

        protected IRetryPolicy RetryPolicy
        {
            get { return _retryPolicy; }
        }

        public TInterface Permanent()
        {
            _referenceFactory = PermanentSubscriptionReference.Create;

            return this as TInterface;
        }

        public TInterface Transient()
        {
            _referenceFactory = TransientSubscriptionReference.Create;

            return this as TInterface;
        }

        public TInterface SetReferenceFactory(ReferenceFactory referenceFactory)
        {
            _referenceFactory = referenceFactory;

            return this as TInterface;
        }

        public TInterface SetRetryPolicy(IRetryPolicy retryPolicy)
        {
            _retryPolicy = retryPolicy ?? Retry.None;

            return this as TInterface;
        }
    }
}