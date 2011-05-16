namespace MassTransit.Transports.RabbitMq.Tests
{
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using TestFramework;


	[Scenario]
	public class When_a_message_is_published :
		Given_a_rabbitmq_bus
	{
		Future<A> _received;
		Future<B> _receivedB;
		UnsubscribeAction _unsubscribe;

		[When]
		public void A_message_is_published()
		{
			_received = new Future<A>();
			_receivedB = new Future<B>();

			var unsub = LocalBus.SubscribeHandler<A>(message => _received.Complete(message));
			_unsubscribe = unsub += LocalBus.SubscribeHandler<B>(message => _receivedB.Complete(message));

			LocalBus.Publish(new A
				{
					StringA = "ValueA",
					StringB = "ValueB",
				});
		}

		[Finally]
		public void Final()
		{
			_unsubscribe();
		}

		[Then]
		public void Should_be_received_by_the_queue()
		{
			_received.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
			_received.Value.StringA.ShouldEqual("ValueA");
		}

		[Then]
		public void Should_receive_the_inherited_version()
		{
			_receivedB.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
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
			_receivedB.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
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
