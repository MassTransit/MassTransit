namespace MassTransit.ServiceBus.Internal
{
	using System.Collections.Generic;

	public class SubscriptionTypeInfo :
		ISubscriptionTypeInfo
	{
		private readonly List<ISubscriptionTypeInfo> _messageTypeSubscriptions = new List<ISubscriptionTypeInfo>();

		public void Add(ISubscriptionTypeInfo subscriptionTypeInfo)
		{
			_messageTypeSubscriptions.Add(subscriptionTypeInfo);
		}

		public void Subscribe<T>(T component) where T : class
		{
			foreach (ISubscriptionTypeInfo subscription in _messageTypeSubscriptions)
			{
				subscription.Subscribe(component);
			}
		}

		public void Unsubscribe<T>(T component) where T : class
		{
			foreach (ISubscriptionTypeInfo subscription in _messageTypeSubscriptions)
			{
				subscription.Unsubscribe(component);
			}
		}

		public void AddComponent()
		{
			foreach (ISubscriptionTypeInfo subscription in _messageTypeSubscriptions)
			{
				subscription.AddComponent();
			}
		}

		public void RemoveComponent()
		{
			foreach (ISubscriptionTypeInfo subscription in _messageTypeSubscriptions)
			{
				subscription.AddComponent();
			}
		}
	}
}