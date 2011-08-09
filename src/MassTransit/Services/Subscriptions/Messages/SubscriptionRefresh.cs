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
namespace MassTransit.Services.Subscriptions.Messages
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SubscriptionRefresh
    {
		private List<SubscriptionInformation> _subscriptions;

    	public SubscriptionRefresh(List<SubscriptionInformation> subscriptions)
    	{
    		_subscriptions = subscriptions;
    	}

    	public SubscriptionRefresh(IEnumerable<SubscriptionInformation> subscriptions)
        {
			_subscriptions = new List<SubscriptionInformation>(subscriptions);
        }

    	protected SubscriptionRefresh()
    	{
    		_subscriptions = new List<SubscriptionInformation>();
    	}

    	public IList<SubscriptionInformation> Subscriptions
        {
            get { return _subscriptions; }
			set { _subscriptions = new List<SubscriptionInformation>(value); }
        }
    }
}