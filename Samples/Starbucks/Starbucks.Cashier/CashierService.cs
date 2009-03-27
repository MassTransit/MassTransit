namespace Starbucks.Cashier
{
	using MassTransit;
	using Microsoft.Practices.ServiceLocation;

	public class CashierService
	{
		private IServiceBus _bus;
		private UnsubscribeAction _unsubscribeAction;

		public void Start()
		{
			_bus = ServiceLocator.Current.GetInstance<IServiceBus>();
			_unsubscribeAction = _bus.Subscribe<FriendlyCashier>();
		}

		public void Stop()
		{
			_unsubscribeAction();
			_bus.Dispose();
		}
	}
}