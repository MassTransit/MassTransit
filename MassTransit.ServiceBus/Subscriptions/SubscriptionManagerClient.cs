namespace MassTransit.ServiceBus.Subscriptions
{
	using System;
	using Messages;

	public class SubscriptionManagerClient :
		IMessageService
	{
		private readonly IServiceBus _serviceBus;
		private readonly ISubscriptionStorage _cache;
		private readonly IMessageQueueEndpoint _managerEndpoint;

		public SubscriptionManagerClient(IServiceBus serviceBus, ISubscriptionStorage cache, IMessageQueueEndpoint managerEndpoint)
		{
			_serviceBus = serviceBus;
			_cache = cache;
			_managerEndpoint = managerEndpoint;
		}

		public void Dispose()
		{
			_serviceBus.Dispose();
		}

		public void Cache_SubscriptionChanged(object sender, SubscriptionChangedEventArgs e)
		{
			if (e.Change.Subscription.Address.Host.ToLowerInvariant() == Environment.MachineName.ToLowerInvariant())
			{
				_serviceBus.Send(_managerEndpoint, e.Change);
			}
		}

		public void HandleSubscriptionChange(IMessageContext<SubscriptionChange> ctx)
		{
			switch (ctx.Message.ChangeType)
			{
				case SubscriptionChangeType.Add:
					_cache.Add(ctx.Message.Subscription.MessageName, ctx.Message.Subscription.Address);
					break;

				case SubscriptionChangeType.Remove:
					_cache.Remove(ctx.Message.Subscription.MessageName, ctx.Message.Subscription.Address);
					break;

				default:
					break;
			}
		}

		public void CacheUpdateResponse_Callback(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
				return;

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
						_cache.Add(sub.MessageName, sub.Address);
					}
				}
			}
		}

		public void Start()
		{
			_cache.SubscriptionChanged += Cache_SubscriptionChanged;
			_serviceBus.Subscribe<SubscriptionChange>(HandleSubscriptionChange);
			IServiceBusAsyncResult asyncResult =
				_serviceBus.Request(_managerEndpoint, CacheUpdateResponse_Callback, this, new CacheUpdateRequest());
		}

		public void Stop()
		{
			_cache.SubscriptionChanged -= Cache_SubscriptionChanged;
			_serviceBus.Send(_managerEndpoint, new CancelSubscriptionUpdates());
			_serviceBus.Unsubscribe<SubscriptionChange>(HandleSubscriptionChange);
		}
	}
}