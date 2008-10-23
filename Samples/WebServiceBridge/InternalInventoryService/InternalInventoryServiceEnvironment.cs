namespace InternalInventoryService
{
    using MassTransit.Host;
    using MassTransit.Host.Configurations;

    public class InternalInventoryServiceEnvironment :
        LocalSystemConfiguration
    {
        private readonly IApplicationLifeCycle _lifeCycle;

        public InternalInventoryServiceEnvironment(string xmlFile)
        {
            _lifeCycle = new InternalInventoryServiceLifeCycle(xmlFile);
        }

        public override string ServiceName
        {
            get { return "InternalInventoryService"; }
        }

        public override string DisplayName
        {
            get { return "Internal Inventory Service"; }
        }

        public override string Description
        {
            get { return "Handles inventory for internal systems"; }
        }

        public override IApplicationLifeCycle LifeCycle
        {
            get { return _lifeCycle; }
        }
    }
}