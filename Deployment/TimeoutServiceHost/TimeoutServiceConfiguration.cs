namespace TimeoutServiceHost
{
    using MassTransit.Host.Configurations;
    using MassTransit.Host.LifeCycles;

    public class TimeoutServiceConfiguration :
        InteractiveConfiguration
    {
        private readonly IApplicationLifeCycle _lifecycle;

        public TimeoutServiceConfiguration(string xmlFile)
        {
            _lifecycle = new TimeoutServiceLifeCycle(xmlFile);
        }

        public override IApplicationLifeCycle LifeCycle
        {
            get { return _lifecycle; }
        }

        public override string ServiceName
        {
            get { return "MT-TIMEOUT"; }
        }

        public override string DisplayName
        {
            get { return "Mass Transit Timeout Service"; }
        }

        public override string Description
        {
            get { return "Think Egg Timer"; }
        }

        public override string[] Dependencies
        {
            get { return new string[] {"MSMQ"}; }
        }
    }
}