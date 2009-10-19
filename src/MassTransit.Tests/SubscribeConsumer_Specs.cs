namespace MassTransit.Tests
{
	using System;
	using Magnum.DateTimeExtensions;
	using Messages;
	using NUnit.Framework;
	using Rhino.Mocks;
	using TextFixtures;

	[TestFixture]
	public class A_subscribed_consumer
		: LoopbackTestFixture
	{
		[Test]
		public void Should_request_an_instance_of_the_consumer_for_each_message()
		{
			var called = new FutureMessage<PingMessage>();

			var ping = new PingMessage();

			var getter = MockRepository.GenerateMock<Func<PingMessage, Action<PingMessage>>>();
			getter.Expect(x => x(ping)).Return(called.Set);

			LocalBus.SubscribeConsumer<PingMessage>(getter);

			LocalBus.Publish(ping);

			called.IsAvailable(3.Seconds()).ShouldBeTrue();
		}
	}
}
