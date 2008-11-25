using MassTransit.Host.LifeCycles;
using Microsoft.Practices.ServiceLocation;

namespace Starbucks.Barista
{
    using MassTransit;

    public class BaristaLifecycle : HostedLifecycle
    {
        public BaristaLifecycle(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
        }

        public override void Start()
        {
            IServiceBus bus = ServiceLocator.GetInstance<IServiceBus>();
            bus.Subscribe<DrinkPreparationSaga>();            
        }

        public override void Stop()
        {
        }
    }
}