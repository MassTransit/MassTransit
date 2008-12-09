// Copyright 2007-2008 The Apache Software Foundation.
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
namespace MassTransit.Subscriptions.Messages
{
    using System;

    [Serializable]
    public abstract class SubscriptionChange
    {
        private Subscription _subscription;

        protected SubscriptionChange()
        {
        }

        protected SubscriptionChange(string messageName, Uri address)
            : this(new Subscription(messageName, address))
        {
        }

        protected SubscriptionChange(Subscription subscription)
        {
            _subscription = subscription;
        }

        public Subscription Subscription
        {
            get { return _subscription; }
            set { _subscription = value; }
        }

    	public bool Equals(SubscriptionChange obj)
    	{
    		if (ReferenceEquals(null, obj)) return false;
    		if (ReferenceEquals(this, obj)) return true;
    		return Equals(obj._subscription, _subscription);
    	}

    	public override bool Equals(object obj)
    	{
    		if (ReferenceEquals(null, obj)) return false;
    		if (ReferenceEquals(this, obj)) return true;
    		if (obj.GetType() != typeof (SubscriptionChange)) return false;
    		return Equals((SubscriptionChange) obj);
    	}

    	public override int GetHashCode()
    	{
    		return (_subscription != null ? _subscription.GetHashCode() : 0);
    	}
    }
}