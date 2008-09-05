namespace MassTransit.ServiceBus.WCF.Tests
{
	using System;
	using MassTransit.ServiceBus.Tests;
	using MassTransit.ServiceBus.Tests.Messages;
	using NUnit.Framework;

	[TestFixture]
	public class When_a_message_is_published :
		LocalAndRemoteTestContext
	{
		protected override string GetCastleConfigurationFile()
		{
			return "wcf.castle.xml";
		}

		private readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

		[Test]
		public void It_should_be_received_by_one_subscribed_consumer()
		{
			TestMessageConsumer<PingMessage> consumer = new TestMessageConsumer<PingMessage>();
			RemoteBus.Subscribe(consumer);

			PingMessage message = new PingMessage();
			LocalBus.Publish(message);

			consumer.ShouldHaveReceivedMessage(message, _timeout);
		}
	}
}