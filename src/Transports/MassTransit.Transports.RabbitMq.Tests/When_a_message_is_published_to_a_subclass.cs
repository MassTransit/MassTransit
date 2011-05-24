namespace MassTransit.Transports.RabbitMq.Tests
{
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using TestFramework;

	[Scenario]
	public class When_a_message_is_published_to_a_subclass :
		Given_a_rabbitmq_bus
	{
		Future<B> _receivedB;

		[When]
		public void A_message_is_published()
		{
			_receivedB = new Future<B>();

			LocalBus.SubscribeHandler<B>(message => _receivedB.Complete(message));

			LocalBus.Publish(new A
				{
					StringA = "ValueA",
					StringB = "ValueB",
				});
		}

		[Then]
		public void Should_receive_the_inherited_version()
		{
			AssertionsForBoolean.ShouldBeTrue(_receivedB.WaitUntilCompleted(8.Seconds()));
			_receivedB.Value.StringB.ShouldEqual("ValueB");
		}

		class A :
			B
		{
			public string StringA { get; set; }
		}

		class B
		{
			public string StringB { get; set; }
		}
	}
}