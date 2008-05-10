namespace CodeCamp.Service
{
	using System;
	using MassTransit.ServiceBus;
	using MassTransit.ServiceBus.MSMQ;
	using MassTransit.ServiceBus.Subscriptions;
	using Messages;

	internal class Program
	{
		private static void Main(string[] args)
		{
			IEndpoint endpoint = new MsmqEndpoint("msmq://localhost/test_server");
			IEndpoint subscriptionsEndpoint = new MsmqEndpoint("msmq://localhost/test_subscriptions");
			ISubscriptionCache cache = new LocalSubscriptionCache();
			
			using (IServiceBus serviceBus = new ServiceBus(endpoint, cache))
			{
				SubscriptionClient client = new SubscriptionClient(serviceBus, cache, subscriptionsEndpoint);
				client.Start();

				serviceBus.Subscribe<UserPasswordFailure>(Handle);
				serviceBus.Subscribe<UserPasswordSuccess>(Handle);

				Console.WriteLine("Service running...");

				Console.ReadKey();

				Console.WriteLine("Service exiting...");

				serviceBus.Unsubscribe<UserPasswordFailure>(Handle);
			}

			Console.WriteLine("End of line");
		}

		private static void Handle(IMessageContext<UserPasswordSuccess> ctx)
		{
			Console.WriteLine("Audit: User password succeeded password check for user {0} at {1}", ctx.Message.Username, ctx.Message.TimeStamp);
			
		}

		private static void Handle(IMessageContext<UserPasswordFailure> ctx)
		{
			Console.WriteLine("Audit: Failed user password check for user {0} at {1}", ctx.Message.Username, ctx.Message.TimeStamp);
		}
	}
}