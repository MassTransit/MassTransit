namespace MassTransit.SubscriptionStorage
{
	using System;
	using MassTransit.ServiceBus.Subscriptions;

	public class PersistentSubscription :
		Subscription
	{
#pragma warning disable 649
		private int _id;
#pragma warning restore 649
		private bool _isActive;

		protected PersistentSubscription()
		{
		}

		public PersistentSubscription(Subscription subscription)
			: base(subscription)
		{
			_isActive = true;
		}

		public int Id
		{
			get { return _id; }
		}

		public bool IsActive
		{
			get { return _isActive; }
			set { _isActive = value; }
		}

		public string Address
		{
			get { return _endpointUri.ToString(); }
			set { _endpointUri = new Uri(value); }
		}
	}
}