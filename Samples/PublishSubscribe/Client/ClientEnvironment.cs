namespace Client
{
    using System;
    using MassTransit.Host;
    using MassTransit.ServiceBus;
    using SecurityMessages;

	public class ClientEnvironment : HostedEnvironment
	{
	    private IServiceBus _bus;
		public ClientEnvironment()
		{
		}

		public ClientEnvironment(string xmlFile) : base(xmlFile)
		{
            //Container.AddComponent<IHostedService, AskPasswordQuestion>();
            Container.AddComponent<PasswordUpdater>();

            _bus = Container.Resolve<IServiceBus>();
		}


	    public override void Main()
	    {
            _bus.AddComponent<PasswordUpdater>();

            Console.WriteLine(new string('-', 20));
            Console.WriteLine("New Password Client");
            Console.WriteLine("What would you like to set your new password to?");
            Console.Write("New Password:");
            string newPassword = Console.ReadLine();

            Console.WriteLine(new string('-', 20));

            RequestPasswordUpdate message = new RequestPasswordUpdate(newPassword);

            _bus.Publish(message);

            Console.WriteLine("Waiting For Reply");
            Console.WriteLine(new string('-', 20));
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
