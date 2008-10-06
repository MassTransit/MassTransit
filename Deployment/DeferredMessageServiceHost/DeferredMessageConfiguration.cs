namespace DeferredMessageServiceHost
{
    using MassTransit.Host.Configurations;
    using MassTransit.Host.LifeCycles;

    public class DeferredMessageConfiguration :
        InteractiveConfiguration
    {
        private IApplicationLifeCycle _lifeCycle;

        public DeferredMessageConfiguration(string xmlFile)
        {
            _lifeCycle = new DeferredMessageLifeCycle(xmlFile);
        }

        public override string ServiceName
        {
            get { return "MT-DEFERRED"; }
        }

        public override string DisplayName
        {
            get { return "Mass Transit Deferred Message Service"; }
        }

        public override string Description
        {
            get { return "Think 'Hold This'"; }
        }

        public override string[] Dependencies
        {
            get { return new string[] {"MSMQ"}; }
        }

        public override IApplicationLifeCycle LifeCycle
        {
            get { return _lifeCycle; }
        }
    }
}