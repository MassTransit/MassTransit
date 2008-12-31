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
namespace MassTransit.Internal
{
	using System;
	using Pipeline;
	using Subscriptions;

	public class SubscriptionCacheEventConnector :
		ISubscriptionEvent
	{
		private readonly ISubscriptionCache _cache;
		private readonly IEndpoint _endpoint;

		public SubscriptionCacheEventConnector(ISubscriptionCache cache, IEndpoint endpoint)
		{
			_cache = cache;
			_endpoint = endpoint;
		}

		public UnsubscribeAction SubscribedTo(Type messageType)
		{
			Subscription subscription = new Subscription(messageType, _endpoint.Uri);

			_cache.Add(subscription);

			return () =>
				{
					_cache.Remove(subscription);
					return true;
				};
		}

		public UnsubscribeAction SubscribedTo(Type messageType, string correlationId)
		{
			Subscription subscription = new Subscription(messageType, correlationId, _endpoint.Uri);

			_cache.Add(subscription);

			return () =>
				{
					_cache.Remove(subscription);
					return true;
				};
		}

		public void UnsubscribedFrom(Type messageType)
		{
			Subscription subscription = new Subscription(messageType, _endpoint.Uri);

			_cache.Remove(subscription);
		}

		public void UnsubscribedFrom(Type messageType, string correlationId)
		{
			Subscription subscription = new Subscription(messageType, correlationId, _endpoint.Uri);

			_cache.Remove(subscription);
		}
	}
}