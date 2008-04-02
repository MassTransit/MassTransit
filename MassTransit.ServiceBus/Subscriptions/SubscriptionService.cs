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
	using System.Collections.Generic;
	using log4net;
	using Messages;

	public class SubscriptionService :
		IMessageService
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (SubscriptionService));
		private readonly IServiceBus _bus;
		private readonly ISubscriptionCache _cache;
		private readonly ISubscriptionRepository _repository;

		public SubscriptionService(IServiceBus bus, ISubscriptionCache subscriptionCache, ISubscriptionRepository subscriptionRepository)
		{
			_bus = bus;
			_cache = subscriptionCache;
			_repository = subscriptionRepository;
		}

		#region IDisposable Members

		public void Dispose()
		{
			_bus.Dispose();
			_cache.Dispose();
			_repository.Dispose();
		}

		#endregion

		public void Start()
		{
			foreach (Subscription sub in _repository.List())
			{
				_cache.Add(sub);
			}

			_bus.Subscribe<CacheUpdateRequest>(HandleCacheUpdateRequest);
			_bus.Subscribe<AddSubscription>(HandleAddSubscription);
			_bus.Subscribe<RemoveSubscription>(HandleRemoveSubscription);
		}

		public void Stop()
		{
			_bus.Unsubscribe<CacheUpdateRequest>(HandleCacheUpdateRequest);
			_bus.Unsubscribe<AddSubscription>(HandleAddSubscription);
			_bus.Unsubscribe<RemoveSubscription>(HandleRemoveSubscription);
		}

		public void HandleAddSubscription(IMessageContext<AddSubscription> ctx)
		{
			try
			{
				_cache.Add(ctx.Message.Subscription);

				_repository.Save(ctx.Message.Subscription);
			}
			catch (Exception ex)
			{
				_log.Error("Exception handling subscription change", ex);
			}
		}

		public void HandleRemoveSubscription(IMessageContext<RemoveSubscription> ctx)
		{
			try
			{
				_cache.Remove(ctx.Message.Subscription);

				_repository.Remove(ctx.Message.Subscription);
			}
			catch (Exception ex)
			{
				_log.Error("Exception handling subscription change", ex);
			}
		}

		public void HandleCacheUpdateRequest(IMessageContext<CacheUpdateRequest> ctx)
		{
			try
			{
				// TODO RegisterSenderForUpdates(ctx.Envelope);

				IList<Subscription> subscriptions = _cache.List();

				CacheUpdateResponse response = new CacheUpdateResponse(subscriptions);

				ctx.Reply(response);
			}
			catch (Exception ex)
			{
				_log.Error("Exception handling cache update request", ex);
			}
		}
	}
}