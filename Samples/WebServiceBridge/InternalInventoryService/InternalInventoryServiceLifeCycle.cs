namespace InternalInventoryService
{
    using MassTransit.Host.Actions;
    using MassTransit.Host.LifeCycles;
    using MassTransit.ServiceBus;

    public class InternalInventoryServiceLifeCycle :
        HostedLifeCycle
    {
        private IServiceBus _bus;

        public InternalInventoryServiceLifeCycle(string xmlFile)
            : base(xmlFile)
        {
        }

        public override NamedAction DefaultAction
        {
            get { return NamedAction.Console; }
        }

        public override void Start()
        {
            Container.AddComponent<InventoryLevelService>();

            _bus = Container.Resolve<IServiceBus>("server");

            _bus.AddComponent<InventoryLevelService>();
        }

        public override void Stop()
        {
            _bus.Dispose();
            Container.Release(_bus);
        }
    }
}