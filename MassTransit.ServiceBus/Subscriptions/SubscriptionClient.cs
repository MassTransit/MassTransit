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
namespace MassTransit.ServiceBus.Subscriptions
{
	using System;
	using System.Collections.Generic;
	using Exceptions;
	using Messages;

	public class SubscriptionClient :
		IHostedService,
		Consumes<CacheUpdateResponse>.All,
		Consumes<AddSubscription>.All,
		Consumes<RemoveSubscription>.All
	{
		private readonly ISubscriptionCache _cache;
		private readonly List<Uri> _localEndpoints = new List<Uri>();
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
			if (!IsOwnedSubscription(message.Subscription))
				_cache.Add(message.Subscription);
		}

		public void Consume(CacheUpdateResponse message)
		{
			foreach (Subscription sub in message.Subscriptions)
			{
				if (!IsOwnedSubscription(sub))
					_cache.Add(sub);
			}

			// to make things good, we need to enumerate the local subscriptions 
			// and add anything that is local in case it was missed
			// during startup

			PublishLocalCacheToServer();
		}

		private void PublishLocalCacheToServer()
		{
			IList<Subscription> subscriptions = _cache.List();
			foreach (Subscription subscription in subscriptions)
			{
				if(IsOwnedSubscription(subscription))
				{
					AddSubscription message = new AddSubscription(subscription);

					_subscriptionServiceEndpoint.Send(message);
				}
			}
		}

		private void RemoveLocalCacheFromServer()
		{
			IList<Subscription> subscriptions = _cache.List();
			foreach (Subscription subscription in subscriptions)
			{
				if(IsOwnedSubscription(subscription))
				{
					RemoveSubscription message = new RemoveSubscription(subscription);

					_subscriptionServiceEndpoint.Send(message);
				}
			}
		}

		public void Consume(RemoveSubscription message)
		{
			if (!IsOwnedSubscription(message.Subscription))
				_cache.Remove(message.Subscription);
		}

		public void Dispose()
		{
			//the bus owns the client so it shouldn't be disposed
			//the bus owns the cache so it shouldn't be disposed
			_subscriptionServiceEndpoint.Dispose();
		}

		public void Start()
		{
			ValidateThatBusAndClientAreNotOnSameEndpoint(_serviceBus, _subscriptionServiceEndpoint);

			_localEndpoints.Add(_serviceBus.Endpoint.Uri);

			_cache.OnAddSubscription += Cache_OnAddSubscription;
			_cache.OnRemoveSubscription += Cache_OnRemoveSubscription;

			_serviceBus.Subscribe(this);

			_subscriptionServiceEndpoint.Send(new CacheUpdateRequest(_serviceBus.Endpoint.Uri));
		}

		public void Stop()
		{
			_subscriptionServiceEndpoint.Send(new CancelSubscriptionUpdates(_serviceBus.Endpoint.Uri));

			_serviceBus.Unsubscribe(this);

			_cache.OnAddSubscription -= Cache_OnAddSubscription;
			_cache.OnRemoveSubscription -= Cache_OnRemoveSubscription;

			RemoveLocalCacheFromServer();
		}

		private bool IsOwnedSubscription(Subscription subscription)
		{
			return _localEndpoints.Contains(subscription.EndpointUri);
		}

		private static void ValidateThatBusAndClientAreNotOnSameEndpoint(IServiceBus bus, IEndpoint endpoint)
		{
			if (bus.Endpoint.Uri.Equals(endpoint.Uri))
			{
				string message = string.Format("Both the service bus and subscription client are listening on the same endpoint {0}", endpoint.Uri);
				throw new EndpointException(endpoint, message);
			}
		}

		public void Cache_OnAddSubscription(object sender, SubscriptionEventArgs e)
		{
			if (IsOwnedSubscription(e.Subscription))
			{
				AddSubscription message = new AddSubscription(e.Subscription);

				_subscriptionServiceEndpoint.Send(message);
			}
		}


		public void Cache_OnRemoveSubscription(object sender, SubscriptionEventArgs e)
		{
			if (IsOwnedSubscription(e.Subscription))
			{
				RemoveSubscription message = new RemoveSubscription(e.Subscription);

				_subscriptionServiceEndpoint.Send(message);
			}
		}

		public void AddLocalEndpoint(IEndpoint endpoint)
		{
			_localEndpoints.Add(endpoint.Uri);
		}
	}
}