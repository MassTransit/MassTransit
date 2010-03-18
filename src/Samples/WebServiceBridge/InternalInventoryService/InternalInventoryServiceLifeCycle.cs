namespace InternalInventoryService
{
	using MassTransit;

    public class InternalInventoryServiceLifeCycle
    {
        private IServiceBus _bus;
    	private UnsubscribeAction _unsubscribeToken;

        public InternalInventoryServiceLifeCycle(IServiceBus bus)
        {
            _bus = bus;
        }

        public void Start()
        {

        	_unsubscribeToken = _bus.Subscribe<InventoryLevelService>();
        }

        public void Stop()
        {
        	_unsubscribeToken();
        }
    }
}