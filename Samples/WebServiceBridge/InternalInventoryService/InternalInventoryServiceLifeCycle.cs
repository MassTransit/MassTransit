namespace InternalInventoryService
{
	using System;
	using MassTransit;
    using MassTransit.Host.Actions;
    using MassTransit.Host.LifeCycles;
    using Microsoft.Practices.ServiceLocation;

    public class InternalInventoryServiceLifeCycle :
        HostedLifecycle
    {
        private IServiceBus _bus;
    	private UnsubscribeAction _unsubscribeToken;

    	public InternalInventoryServiceLifeCycle(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
        }

        public override NamedAction DefaultAction
        {
            get { return NamedAction.Console; }
        }

        public override void Start()
        {
            _bus = this.ServiceLocator.GetInstance<IServiceBus>("server");

        	_unsubscribeToken = _bus.Subscribe<InventoryLevelService>();
        }

        public override void Stop()
        {
        	_unsubscribeToken();
        }
    }
}