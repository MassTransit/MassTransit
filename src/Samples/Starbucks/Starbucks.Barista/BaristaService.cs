namespace Starbucks.Barista
{
    using MassTransit;

    public class BaristaService
    {
    	private readonly IServiceBus _bus;
    	private UnsubscribeAction _unsubscribeAction;

        public BaristaService(IServiceBus bus)
        {
            _bus = bus;
        }

        public void Start()
        {
    		_unsubscribeAction = _bus.Subscribe<DrinkPreparationSaga>();
        }

        public void Stop()
        {
        	_unsubscribeAction();
        	_bus.Dispose();
        }
    }
}