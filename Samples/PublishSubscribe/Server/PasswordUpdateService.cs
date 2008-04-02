namespace Server
{
	using System;
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

		//private static void Main(string[] args)
		//{
		//    //IMessageQueueEndpoint serverEndpoint = new MessageQueueEndpoint("msmq://localhost/test_server");
		//    //IMessageQueueEndpoint subscriptionManagerEndpoint = new MessageQueueEndpoint("msmq://localhost/test_subscriptions");

		//    //ISubscriptionStorage storage = new LocalSubscriptionCache();

		//    //ServiceBus bus = new ServiceBus(serverEndpoint, storage);
		//    //SubscriptionManagerClient subscriptionClient = new SubscriptionManagerClient(bus, storage, subscriptionManagerEndpoint);
		//    //subscriptionClient.Start();
		//}

		#region IMessageService Members

		public void Dispose()
		{
			_serviceBus.Dispose();
		}

		public void Start()
		{
			_serviceBus.Subscribe<RequestPasswordUpdate>(RequestPasswordUpdate_Received);
		}

		public void Stop()
		{
			_serviceBus.Unsubscribe<RequestPasswordUpdate>(RequestPasswordUpdate_Received);
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