using MassTransit.Host.LifeCycles;
using MassTransit.ServiceBus;
using Microsoft.Practices.ServiceLocation;

namespace Starbucks.Cashier
{
    public class CashierLifecycle : HostedLifeCycle
    {
        public CashierLifecycle(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
        }

        public override void Start()
        {
            IServiceBus bus = ServiceLocator.GetInstance<IServiceBus>();            
            bus.AddComponent<EmoCollegeDropout>();
        }

        public override void Stop()
        {
        }
    }
}