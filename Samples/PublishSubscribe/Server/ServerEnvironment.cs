namespace Server
{
    using MassTransit.Host;
    using MassTransit.Host.Configurations;
    using MassTransit.Host.LifeCycles;

    public class ServerEnvironment :
		LocalSystemConfiguration
	{
        private IApplicationLifeCycle _lifeCycle;

		public ServerEnvironment(string xmlFile)
		{
		    _lifeCycle = new ServerLifeCycle(xmlFile);
		}

	    public override string ServiceName
		{
			get { return "SampleServerService"; }
		}

		public override string DisplayName
		{
			get { return "MassTransit Sample Server Service"; }
		}

		public override string Description
		{
			get { return "Acts as a server on the service bus"; }
		}

	    public override IApplicationLifeCycle LifeCycle
	    {
            get { return _lifeCycle; }
	    }
	}
}