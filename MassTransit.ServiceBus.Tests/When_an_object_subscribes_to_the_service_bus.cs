namespace MassTransit.ServiceBus.Tests
{
	using System;
	using System.Threading;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class When_an_object_subscribes_to_the_service_bus :
        Specification
	{
        protected override void Before_each()
        {
            mockServiceBusEndPoint = DynamicMock<IEndpoint>();
            _serviceBus = new ServiceBus(mockServiceBusEndPoint);

            SetupResult.For(mockServiceBusEndPoint.Uri).Return(new Uri("msmq://localhost/test_servicebus"));

            ReplayAll();
        }

	    protected override void After_each()
        {
			_serviceBus.Dispose();


			_serviceBus = null;
			mockServiceBusEndPoint = null;
            
        }

/*
        [Test]
        public void An_Event_Handler_Should_Be_Called()
        {
            using (mocks.Record())
            {
                Expect.Call(mockServiceBusEndPoint.Receiver).Return(mockReceiver);
                Expect.Call(delegate { mockReceiver.Subscribe(null); }).IgnoreArguments().Repeat.Any();
            }
            using (mocks.Playback())
            {

                Action<IMessageContext<PingMessage>> handler = delegate { _received = true; };
                _serviceBus.Subscribe(handler);

                _serviceBus.Deliver(new Envelope(mockServiceBusEndPoint, new PingMessage()));

                Assert.That(_received, Is.True);
            }
        }

        [Test]
        public void If_there_are_no_subscriptions_the_message_should_be_ignored()
        {
            using (mocks.Record())
            {
                
            }
            using (mocks.Playback())
            {
                IEnvelope envelope = new Envelope(mockServiceBusEndPoint, new PingMessage());
                _serviceBus.Deliver(envelope);

                //the test is kind of that no errors happened.

            }
        }
    }
}*/

		[Test, Explicit]
		public void Messages_should_be_delivered_for_a_consumer()
		{
			CorrelatedController controller = new CorrelatedController(_serviceBus);
		}

		private ServiceBus _serviceBus;
		private IEndpoint mockServiceBusEndPoint;


        
        internal class CorrelatedController : Consumes<SimpleResponseMessage>.For<Guid>
		{
			private readonly IServiceBus _bus;
			private readonly Guid _id;
			private readonly ManualResetEvent _responseEvent = new ManualResetEvent(false);


			public CorrelatedController(IServiceBus bus)
			{
				_bus = bus;

				_id = Guid.NewGuid();
			}

			public void Consume(SimpleResponseMessage message)
			{
				_responseEvent.Set();
			}

			public Guid CorrelationId
			{
				get { return _id; }
			}

			public event Action<CorrelatedController> OnSuccess = delegate { };
			public event Action<CorrelatedController> OnTimeout = delegate { };

			public void SimulateRequestResponse()
			{
				_bus.Subscribe(this);

				_bus.Publish(new SimpleRequestMessage(_id));

				bool success = _responseEvent.WaitOne(TimeSpan.FromSeconds(5), true);
				if (success)
					OnSuccess(this);
				else
					OnTimeout(this);
			}
		}
		internal class SimpleRequestMessage : CorrelatedBy<Guid>
		{
			private readonly Guid _id;

			public SimpleRequestMessage(Guid id)
			{
				_id = id;
			}

			public Guid CorrelationId
			{
				get { return _id; }
			}
		}
		[Serializable]
		internal class SimpleResponseMessage : CorrelatedBy<Guid>
		{
			private readonly Guid _id;

			//TODO: Remove:Stupid XML Serializer
			public SimpleResponseMessage()
			{
			}

			public SimpleResponseMessage(Guid id)
			{
				_id = id;
			}

			public Guid CorrelationId
			{
				get { return _id; }
			}
		}
	}
}