namespace MassTransit.ServiceBus.MSMQ.Tests

{
	using System;
	using MassTransit.ServiceBus.Tests;
	using MassTransit.ServiceBus.Tests.Messages;
	using NUnit.Framework;

	[TestFixture]
	public class When_a_message_is_published_to_a_transactional_queue :
		LocalAndRemoteTestContext
	{
		protected override string GetCastleConfigurationFile()
		{
			return "transactional.castle.xml";
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

		[Test]
		public void It_should_leave_the_message_in_the_queue_if_an_exception_is_thrown()
		{
			RemoteBus.Subscribe<PingMessage>(m => { throw new ApplicationException("Boing!"); });

			PingMessage message = new PingMessage();
			LocalBus.Publish(message);
		}
	}
}