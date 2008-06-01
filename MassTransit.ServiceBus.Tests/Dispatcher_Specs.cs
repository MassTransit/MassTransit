namespace MassTransit.ServiceBus.Tests
{
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;

	[TestFixture]
	public class When_a_message_is_delivered_to_the_service_bus :
		Specification
	{
		[Test]
		public void A_consumer_object_should_receive_the_message()
		{
			ReplayAll();

			PingHandler handler = new PingHandler();

			_bus.Subscribe(handler);

			int old = PingHandler.Pinged;

			_bus.Dispatch(new PingMessage());

			Assert.That(PingHandler.Pinged, Is.GreaterThan(old));
		}

		[Test]
		public void A_consumer_type_should_be_created_to_receive_the_message()
		{
			ReplayAll();

			_bus.AddComponent<PingHandler>();

			int old = PingHandler.Pinged;

			_bus.Dispatch(new PingMessage());

			Assert.That(PingHandler.Pinged, Is.GreaterThan(old));
		}

		protected override void Before_each()
		{
			IEndpoint endpoint = Mock<IEndpoint>();

			_bus = new ServiceBus(endpoint);
		}

		private ServiceBus _bus;

		internal class PingHandler : Consumes<PingMessage>.All
		{
			private static int _pinged;

			public static int Pinged
			{
				get { return _pinged; }
			}

			public void Consume(PingMessage message)
			{
				_pinged++;
			}
		}
	}
}