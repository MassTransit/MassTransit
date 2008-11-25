using MassTransit.Host.LifeCycles;
using Microsoft.Practices.ServiceLocation;

namespace Starbucks.Cashier
{
    using MassTransit;

    public class CashierLifecycle : HostedLifecycle
    {
        public CashierLifecycle(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
        }

        public override void Start()
        {
            IServiceBus bus = ServiceLocator.GetInstance<IServiceBus>();            
            bus.Subscribe<FriendlyCashier>();
        }

        public override void Stop()
        {
        }
    }
}