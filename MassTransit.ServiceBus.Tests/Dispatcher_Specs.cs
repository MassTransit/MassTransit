namespace MassTransit.ServiceBus.Tests
{
	using Internal;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;

	[TestFixture]
	public class When_a_message_is_delivered_to_the_service_bus :
		Specification
	{
		protected override void Before_each()
		{
			IEndpoint endpoint = Mock<IEndpoint>();
			SetupResult.For(endpoint.Receiver).Return(Stub<IMessageReceiver>());

			_bus = new ServiceBus(endpoint);
		}

		[Test]
		public void A_consumer_object_should_receive_the_message()
		{
			ReplayAll();

			PingHandler handler = new PingHandler();

			_bus.Subscribe(handler);

			int old = PingHandler.Pinged;
			
			_bus.Deliver(new Envelope(new PingMessage()));

			Assert.That(PingHandler.Pinged, Is.GreaterThan(old));
		}

		[Test]
		public void A_consumer_type_should_be_created_to_receive_the_message()
		{
			ReplayAll();

			_bus.AddComponent<PingHandler>();

			int old = PingHandler.Pinged;
			
			_bus.Deliver(new Envelope(new PingMessage()));

			Assert.That(PingHandler.Pinged, Is.GreaterThan(old));
		}

		private ServiceBus _bus;

		internal class PingHandler : Consumes<PingMessage>.Any
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