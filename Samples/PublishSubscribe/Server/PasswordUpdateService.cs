namespace Server
{
	using System;
	using System.IO;
	using MassTransit.ServiceBus;
	using SecurityMessages;

	public class PasswordUpdateService :
		IHostedService,
        Consumes<RequestPasswordUpdate>.All
	{
		private readonly IServiceBus _serviceBus;

		public PasswordUpdateService(IServiceBus serviceBus)
		{
			_serviceBus = serviceBus;
		}

		#region IMessageService Members

		public void Dispose()
		{
			_serviceBus.Dispose();
		}

		public void Start()
		{
			log4net.Config.XmlConfigurator.Configure(new FileInfo("server.log4net.xml"));
			_serviceBus.Subscribe(this);
		}

		public void Stop()
		{
			//don't unsubscribe
			//just because I stopped doesn't mean I want to stop getting messages
		}

		#endregion

	    public void Consume(RequestPasswordUpdate message)
	    {
			Console.WriteLine(new string('-', 20));
			Console.WriteLine("Received Message");
			Console.WriteLine(message.NewPassword);
			Console.WriteLine(new string('-', 20));
			_serviceBus.Publish(new PasswordUpdateComplete(0));
	    }
	}
}