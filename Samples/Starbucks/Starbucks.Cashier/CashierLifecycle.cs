namespace Starbucks.Cashier
{
    using MassTransit;
    using Microsoft.Practices.ServiceLocation;

    public class CashierLifecycle 
    {
        public void Start()
        {
            IServiceBus bus = ServiceLocator.Current.GetInstance<IServiceBus>();
            bus.Subscribe<FriendlyCashier>();
        }

        public void Stop()
        {
        }
    }
}