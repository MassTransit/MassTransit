/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.Subscriptions
{
	using System;
	using Messages;

	public class SubscriptionClient :
		IHostedService, 
		Consumes<AddSubscription>.Any, 
		Consumes<RemoveSubscription>.Any, 
		Consumes<CacheUpdateResponse>.Any
	{
		private readonly ISubscriptionCache _cache;
		private readonly IServiceBus _serviceBus;
		private readonly IEndpoint _subscriptionServiceEndpoint;

		public SubscriptionClient(IServiceBus serviceBus, ISubscriptionCache cache, IEndpoint subscriptionServiceEndpoint)
		{
			_serviceBus = serviceBus;
			_cache = cache;
			_subscriptionServiceEndpoint = subscriptionServiceEndpoint;
		}

		public void Consume(AddSubscription message)
		{
			_cache.Add(message.Subscription);
		}

		public void Consume(CacheUpdateResponse message)
		{
			foreach (Subscription sub in message.Subscriptions)
			{
				_cache.Add(sub);
			}
		}

		public void Consume(RemoveSubscription message)
		{
			_cache.Add(message.Subscription);
		}

		public void Dispose()
		{
			_serviceBus.Dispose(); //TODO: Do we want to do this? - dds
		}

		public void Start()
		{
			_cache.OnAddSubscription += Cache_OnAddSubscription;
			_cache.OnRemoveSubscription += Cache_OnRemoveSubscription;

			_serviceBus.Subscribe(this);

			_subscriptionServiceEndpoint.Send(new CacheUpdateRequest());
		}

		public void Stop()
		{
			_subscriptionServiceEndpoint.Send(new CancelSubscriptionUpdates());

			_serviceBus.Unsubscribe(this);

			_cache.OnAddSubscription -= Cache_OnAddSubscription;
			_cache.OnRemoveSubscription -= Cache_OnRemoveSubscription;
		}

		public void Cache_OnAddSubscription(object sender, SubscriptionEventArgs e)
		{
			if (e.Subscription.EndpointUri.Host.ToLowerInvariant() == Environment.MachineName.ToLowerInvariant())
			{
				AddSubscription message = new AddSubscription(e.Subscription);

				_subscriptionServiceEndpoint.Send(message);
			}
		}

		public void Cache_OnRemoveSubscription(object sender, SubscriptionEventArgs e)
		{
			if (e.Subscription.EndpointUri.Host.ToLowerInvariant() == Environment.MachineName.ToLowerInvariant())
			{
				RemoveSubscription message = new RemoveSubscription(e.Subscription);

				_subscriptionServiceEndpoint.Send(message);
			}
		}
	}
}