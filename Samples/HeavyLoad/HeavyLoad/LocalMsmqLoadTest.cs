namespace HeavyLoad
{
    using System;
    using MassTransit.ServiceBus.MSMQ;

	public class LocalMsmqLoadTest : LocalLoadTest
	{
		private static readonly string _queueUri = "msmq://localhost/test_servicebus";

		public LocalMsmqLoadTest()
			: base(new Uri(_queueUri))
		{
			MsmqHelper.ValidateAndPurgeQueue(((MsmqEndpoint) LocalEndpoint).QueuePath);
		}
	}

	public class LocalActiveMqLoadTest : LocalLoadTest
	{
		private static readonly string _queueUri = "activemq://localhost:61616/load_test_queue";

		public LocalActiveMqLoadTest()
			: base(new Uri(_queueUri))
		{
		}
	}
}