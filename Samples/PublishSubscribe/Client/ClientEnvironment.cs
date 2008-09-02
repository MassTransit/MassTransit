namespace Client
{
    using MassTransit.Host.Configurations;
    using MassTransit.Host.LifeCycles;

    public class ClientEnvironment :
        LocalSystemConfiguration
    {
        private readonly IApplicationLifeCycle _lifeCycle;

        public ClientEnvironment(string xmlFile)
        {
            _lifeCycle = new ClientLifeCycle(xmlFile);
        }

        public override string ServiceName
        {
            get { return "SampleClientService"; }
        }

        public override string DisplayName
        {
            get { return "MassTransit Sample Client Service"; }
        }

        public override string Description
        {
            get { return "Acts as a client on the service bus"; }
        }

        public override IApplicationLifeCycle LifeCycle
        {
            get { return _lifeCycle; }
        }
    }
}