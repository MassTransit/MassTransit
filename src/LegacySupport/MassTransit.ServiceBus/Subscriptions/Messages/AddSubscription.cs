using System;

namespace MassTransit.ServiceBus.Subscriptions.Messages
{
	[Serializable]
	public class AddSubscription :
		SubscriptionChange
	{
		protected AddSubscription()
		{
		}

		public AddSubscription(string messageName, Uri address)
			: base(messageName, address)
		{
		}

		public AddSubscription(Subscription subscription)
			: base(subscription)
		{
		}
	}
}