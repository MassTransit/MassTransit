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
	using Util;

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

		#region IMessageService Members

		public void Dispose()
		{
			_serviceBus.Dispose();
		}

		public void Start()
		{
			_cache.OnAddSubscription += Cache_OnAddSubscription;
			_cache.OnRemoveSubscription += Cache_OnRemoveSubscription;

			_serviceBus.Subscribe<AddSubscription>(HandleAddSubscription);
			_serviceBus.Subscribe<RemoveSubscription>(HandleRemoveSubscription);
			_serviceBus.Request(_subscriptionServiceEndpoint, CacheUpdateResponse_Callback, this, new CacheUpdateRequest());
		}

		public void Stop()
		{
			_cache.OnAddSubscription -= Cache_OnAddSubscription;
			_cache.OnRemoveSubscription -= Cache_OnRemoveSubscription;

			_serviceBus.Send(_subscriptionServiceEndpoint, new CancelSubscriptionUpdates());
			_serviceBus.Unsubscribe<AddSubscription>(HandleAddSubscription);
			_serviceBus.Unsubscribe<RemoveSubscription>(HandleRemoveSubscription);
		}

		#endregion

		public void HandleAddSubscription(IMessageContext<AddSubscription> ctx)
		{
			_cache.Add(ctx.Message.Subscription);
		}
		public void HandleRemoveSubscription(IMessageContext<RemoveSubscription> ctx)
		{
			_cache.Add(ctx.Message.Subscription);
		}

		public void CacheUpdateResponse_Callback(IAsyncResult asyncResult)
		{
			Check.Parameter(asyncResult).IsNotNull();

			IServiceBusAsyncResult serviceBusAsyncResult = asyncResult as IServiceBusAsyncResult;
			if (serviceBusAsyncResult == null)
				return;

			if (serviceBusAsyncResult.Messages == null)
				return;

			foreach (IMessage message in serviceBusAsyncResult.Messages)
			{
				CacheUpdateResponse response = message as CacheUpdateResponse;
				if (response != null)
				{
					foreach (Subscription sub in response.Subscriptions)
					{
						_cache.Add(sub);
					}
				}
			}
		}

		public void Cache_OnAddSubscription(object sender, SubscriptionEventArgs e)
		{
			if (e.Subscription.EndpointUri.Host.ToLowerInvariant() == Environment.MachineName.ToLowerInvariant())
			{
				AddSubscription message = new AddSubscription(e.Subscription);

				_serviceBus.Send(_subscriptionServiceEndpoint, message);
			}
		}

		public void Cache_OnRemoveSubscription(object sender, SubscriptionEventArgs e)
		{
			if (e.Subscription.EndpointUri.Host.ToLowerInvariant() == Environment.MachineName.ToLowerInvariant())
			{
				RemoveSubscription message = new RemoveSubscription(e.Subscription);

				_serviceBus.Send(_subscriptionServiceEndpoint, message);
			}
		}
	}
}