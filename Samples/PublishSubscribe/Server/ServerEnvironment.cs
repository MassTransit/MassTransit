namespace Server
{
	using MassTransit.Host2;

	public class ServerEnvironment :
		HostedEnvironment
	{
		public ServerEnvironment()
		{
		}

		public ServerEnvironment(string xmlFile) : base(xmlFile)
		{
		}


	    public override void Main()
	    {
	        //do nothing
	    }

	    public override string ServiceName
		{
			get { return "SampleServerService"; }
		}

		public override string DispalyName
		{
			get { return "MassTransit Sample Server Service"; }
		}

		public override string Description
		{
			get { return "Acts as a server on the service bus"; }
		}
	}
}