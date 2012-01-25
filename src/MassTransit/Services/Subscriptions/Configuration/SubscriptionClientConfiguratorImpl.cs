// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Services.Subscriptions.Configuration
{
	using System;
	using Client;
	using Magnum.Extensions;
	using MassTransit.Subscriptions.Coordinator;

	public class SubscriptionClientConfiguratorImpl :
		SubscriptionClientConfigurator
	{
		Uri _subscriptionServiceUri;
		TimeSpan _timeout = 1.Minutes();


		public void SetSubscriptionServiceEndpoint(Uri uri)
		{
			_subscriptionServiceUri = uri;
		}

		public void SetStartTimeout(TimeSpan timeout)
		{
			_timeout = timeout;
		}

		public SubscriptionObserver Create(IServiceBus bus, SubscriptionRouter router)
		{
			var client = new SubscriptionClient(bus, router, _subscriptionServiceUri, _timeout);
			return client;
		}
	}
}