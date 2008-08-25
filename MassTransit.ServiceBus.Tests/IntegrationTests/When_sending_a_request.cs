namespace MassTransit.ServiceBus.Tests.IntegrationTests
{
	using Messages;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	[Explicit]
	public class When_sending_a_request
	{
		[Test]
		public void A_response_should_release_the_waiting_process()
		{
			using (QueueTestContext qtc = new QueueTestContext())
			{
				PingMessage ping = new PingMessage();

				qtc.RemoteServiceBus.Subscribe<PingMessage>(
					delegate(IMessageContext<PingMessage> context) { context.Reply(new PongMessage()); });

				qtc.RemoteServiceBus.Endpoint.Send(ping);

				PongMessage pong = null;

				Assert.That(pong, Is.Not.Null);
			}
		}
	}
}