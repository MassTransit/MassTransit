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

		[When]
		public void A_message_is_published()
		{
			_received = new Future<A>();
			_receivedB = new Future<B>();

			LocalBus.SubscribeHandler<A>(message => _received.Complete(message));
			LocalBus.SubscribeHandler<B>(message => _receivedB.Complete(message));

			LocalBus.Publish(new A());
		}

		[Then]
		public void Should_be_received_by_the_queue()
		{
			_received.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
		}

		[Then]
		public void Should_receive_the_inherited_version()
		{
			_receivedB.WaitUntilCompleted(8.Seconds()).ShouldBeTrue();
		}

		class A :
			B
		{
			
		}

		class B
		{
			
		}
	}
}
