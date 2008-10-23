namespace InternalInventoryService
{
    using MassTransit.Host.Actions;
    using MassTransit.Host.LifeCycles;
    using MassTransit.ServiceBus;
    using Microsoft.Practices.ServiceLocation;

    public class InternalInventoryServiceLifeCycle :
        HostedLifeCycle
    {
        private IServiceBus _bus;

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

            _bus.AddComponent<InventoryLevelService>();
        }

        public override void Stop()
        {
        }
    }
}