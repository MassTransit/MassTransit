namespace Starbucks.Barista
{
    using MassTransit;
    using Microsoft.Practices.ServiceLocation;

    public class BaristaService
    {
    	private IServiceBus _bus;
    	private UnsubscribeAction _unsubscribeAction;

    	public void Start()
        {
    		_bus = ServiceLocator.Current.GetInstance<IServiceBus>();
    		_unsubscribeAction = _bus.Subscribe<DrinkPreparationSaga>();
        }

        public void Stop()
        {
        	_unsubscribeAction();
        	_bus.Dispose();
        }
    }
}