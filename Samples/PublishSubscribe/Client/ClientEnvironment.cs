namespace Client
{
	using MassTransit.Host2;
    using MassTransit.ServiceBus;
    using SecurityMessages;

	public class ClientEnvironment : HostedEnvironment
	{
		public ClientEnvironment()
		{
		}

		public ClientEnvironment(string xmlFile) : base(xmlFile)
		{
            Container.AddComponent<IHostedService, AskPasswordQuestion>();
            Container.AddComponent<PasswordUpdater>();

            IServiceBus bus = Container.Resolve<IServiceBus>();
		}

		public override string ServiceName
		{
			get { return "SampleClientService"; }
		}

		public override string DispalyName
		{
			get { return "MassTransit Sample Client Service"; }
		}

		public override string Description
		{
			get { return "Acts as a client on the service bus"; }
		}
	}
}
