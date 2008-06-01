namespace CodeCamp.Service
{
	using System;
	using MassTransit.DistributedSubscriptionCache;
	using MassTransit.ServiceBus;
	using MassTransit.ServiceBus.MSMQ;
	using MassTransit.ServiceBus.Subscriptions;
	using Messages;

	internal class Program :
		Consumes<UserPasswordSuccess>.All,
		Consumes<UserPasswordFailure>.All
	{
		public void Consume(UserPasswordFailure message)
		{
			Console.WriteLine("Audit: Failed user password check for user {0} at {1}", message.Username, message.TimeStamp);
		}

		public void Consume(UserPasswordSuccess message)
		{
			Console.WriteLine("Audit: User password succeeded password check for user {0} at {1}", message.Username, message.TimeStamp);
		}

		private static void Main()
		{
			IEndpoint endpoint = new MsmqEndpoint("msmq://localhost/test_server");
			ISubscriptionCache cache = new DistributedSubscriptionCache();

			using (IServiceBus serviceBus = new ServiceBus(endpoint, cache))
			{
				serviceBus.AddComponent<Program>();

				Console.WriteLine("Service running...");

				Console.ReadKey();

				Console.WriteLine("Service exiting...");

				serviceBus.RemoveComponent<Program>();
			}

			Console.WriteLine("End of line");
		}
	}
}