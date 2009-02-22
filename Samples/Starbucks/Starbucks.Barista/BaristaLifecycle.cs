namespace Starbucks.Barista
{
    using MassTransit;
    using Microsoft.Practices.ServiceLocation;

    public class BaristaLifecycle
    {
        public void Start()
        {
            IServiceBus bus = ServiceLocator.Current.GetInstance<IServiceBus>();
            bus.Subscribe<DrinkPreparationSaga>();            
        }

        public void Stop()
        {
        }
    }
}