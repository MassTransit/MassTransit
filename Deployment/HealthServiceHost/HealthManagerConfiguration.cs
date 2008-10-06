namespace HealthServiceHost
{
    using MassTransit.Host.Configurations;
    using MassTransit.Host.LifeCycles;

    public class HealthManagerConfiguration :
        InteractiveConfiguration
    {

        private IApplicationLifeCycle _lifecycle;

        public HealthManagerConfiguration(string xmlFile) 
        {
            _lifecycle = new HealthLifeCycle(xmlFile);
        }

        public override IApplicationLifeCycle LifeCycle
        {
            get { return _lifecycle; }
        }

        public override string ServiceName
        {
            get { return "MassTransit Health Manager"; }
        }

        public override string DisplayName
        {
            get { return "MassTransit Health Manager"; }
        }

        public override string Description
        {
            get { return "This service manages the health for Mass Transit"; }
        }
    }
}