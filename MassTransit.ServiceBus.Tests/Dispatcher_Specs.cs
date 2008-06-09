namespace MassTransit.ServiceBus.Tests
{
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;

    [TestFixture]
	public class When_a_message_is_delivered_to_the_service_bus :
		Specification
	{

		private ServiceBus _bus;
		    IObjectBuilder obj;

		protected override void Before_each()
		{
			IEndpoint endpoint = DynamicMock<IEndpoint>();
		    obj = StrictMock<IObjectBuilder>();

		    _bus = new ServiceBus(endpoint, obj);
		}

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
            PingHandler ph = new PingHandler();

		    Expect.Call(obj.Build<Consumes<PingMessage>.All>(typeof (PingHandler))).Return(ph);
            obj.Release<Consumes<PingMessage>.All>(ph);

			ReplayAll();

			_bus.AddComponent<PingHandler>();

			int old = PingHandler.Pinged;

			_bus.Dispatch(new PingMessage());

			Assert.That(PingHandler.Pinged, Is.GreaterThan(old));
		}


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