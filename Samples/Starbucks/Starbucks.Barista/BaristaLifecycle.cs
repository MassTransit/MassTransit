using MassTransit.Host.LifeCycles;
using MassTransit.ServiceBus;
using Microsoft.Practices.ServiceLocation;

namespace Starbucks.Barista
{
    public class BaristaLifecycle : HostedLifeCycle
    {
        public BaristaLifecycle(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
        }

        public override void Start()
        {
            IServiceBus bus = ServiceLocator.GetInstance<IServiceBus>();
            bus.AddComponent<DrinkPreparationSaga>();            
        }

        public override void Stop()
        {
        }
    }
}