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
    using ClientHandlers;
    using Exceptions;
    using Messages;

	public class SubscriptionClient :
		IHostedService
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

		public void Dispose()
		{
			//the bus owns the client so it shouldn't be disposed
			//the bus owns the cache so it shouldn't be disposed
			_subscriptionServiceEndpoint.Dispose();
		}

		public void Start()
		{
            ValidateThatBusAndClientAreNotOnSameEndpoint(_serviceBus, _subscriptionServiceEndpoint);

			_cache.OnAddSubscription += Cache_OnAddSubscription;
			_cache.OnRemoveSubscription += Cache_OnRemoveSubscription;

            _serviceBus.AddComponent<ClientHandlers.AddSubscriptionHandler>();
            _serviceBus.AddComponent<ClientHandlers.RemoveSubscriptionHandler>();
            _serviceBus.AddComponent<ClientHandlers.CacheUpdateHandler>();

			_subscriptionServiceEndpoint.Send(new CacheUpdateRequest(_serviceBus.Endpoint.Uri));
		}

        private static void ValidateThatBusAndClientAreNotOnSameEndpoint(IServiceBus bus, IEndpoint endpoint)
        {
            if (bus.Endpoint.Uri.Equals(endpoint.Uri))
            {
                string message = string.Format("Both the service bus and subscription client are listening on the same endpoint {0}", endpoint.Uri);
                throw new EndpointException(endpoint, message);
            }
        }

		public void Stop()
		{
			_subscriptionServiceEndpoint.Send(new CancelSubscriptionUpdates(_serviceBus.Endpoint.Uri));

            _serviceBus.RemoveComponent<ClientHandlers.AddSubscriptionHandler>();
            _serviceBus.RemoveComponent<ClientHandlers.RemoveSubscriptionHandler>();
            _serviceBus.RemoveComponent<ClientHandlers.CacheUpdateHandler>();

			_cache.OnAddSubscription -= Cache_OnAddSubscription;
			_cache.OnRemoveSubscription -= Cache_OnRemoveSubscription;
		}

		public void Cache_OnAddSubscription(object sender, SubscriptionEventArgs e)
		{
			if (ClientUtil.IsOwnedSubscription(e.Subscription, _serviceBus))
			{
				AddSubscription message = new AddSubscription(e.Subscription);

				_subscriptionServiceEndpoint.Send(message);
			}
		}

		

		public void Cache_OnRemoveSubscription(object sender, SubscriptionEventArgs e)
		{
            if (ClientUtil.IsOwnedSubscription(e.Subscription, _serviceBus))
			{
				RemoveSubscription message = new RemoveSubscription(e.Subscription);

				_subscriptionServiceEndpoint.Send(message);
			}
		}
	}
}