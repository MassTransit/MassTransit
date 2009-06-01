using System;

namespace MassTransit.ServiceBus.Subscriptions.Messages
{
	[Serializable]
	public class RemoveSubscription :
		SubscriptionChange
	{
		protected RemoveSubscription()
		{
		}

		public RemoveSubscription(string messageName, Uri address)
			: base(messageName, address)
		{
		}

		public RemoveSubscription(Subscription subscription)
			: base(subscription)
		{
		}
	}
}