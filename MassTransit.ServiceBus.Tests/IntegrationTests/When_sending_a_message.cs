namespace MassTransit.ServiceBus.Tests.IntegrationTests
{
	using System;
	using NUnit.Framework;

	[TestFixture]
	[Explicit]
	public class When_sending_a_message
	{
		[Test]
		public void NAME()
		{
			//  ServiceBus bus = new ServiceBus(new MessageQueueEndpoint("msmq://localhost/test_client"), new LocalSubscriptionCache());
			//   bus.Send(new MessageQueueEndpoint("msmq://localhost/test"), new PingMessage());

			throw new NotImplementedException();
		}

		[Test]
		[Ignore("No Test")]
		public void The_message_should_be_delivered_to_a_local_subscriber()
		{
		}
	}
}