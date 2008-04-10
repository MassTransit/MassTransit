namespace Server
{
	using System;
	using System.IO;
	using MassTransit.ServiceBus;
	using SecurityMessages;

	public class PasswordUpdateService :
		IMessageService
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
			_serviceBus.Subscribe<RequestPasswordUpdate>(RequestPasswordUpdate_Received);
		}

		public void Stop()
		{
			//don't unsubscribe
            //just because I stopped doesn't mean I want to stop getting messages
		}

		#endregion

		private static void RequestPasswordUpdate_Received(IMessageContext<RequestPasswordUpdate> cxt)
		{
			Console.WriteLine("Received Message");
			Console.WriteLine(cxt.Message.NewPassword);
			cxt.Reply(new PasswordUpdateComplete(0));
		}
	}
}