namespace InternalInventoryService
{
	using MassTransit;
    using Microsoft.Practices.ServiceLocation;

    public class InternalInventoryServiceLifeCycle
    {
        private IServiceBus _bus;
    	private UnsubscribeAction _unsubscribeToken;


        public void Start()
        {
            _bus = ServiceLocator.Current.GetInstance<IServiceBus>("server");

        	_unsubscribeToken = _bus.Subscribe<InventoryLevelService>();
        }

        public void Stop()
        {
        	_unsubscribeToken();
        }
    }
}