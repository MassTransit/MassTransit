namespace MassTransit.ServiceBus.Tests
{
	using System;
	using Internal;
	using MassTransit.ServiceBus.Subscriptions;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;

	[TestFixture]
	public class When_a_correlated_message_is_received :
        Specification
	{
	    private IObjectBuilder obj;

        protected override void Before_each()
        {
            obj = StrictMock<IObjectBuilder>();
        }
		[Test]
		public void A_type_should_be_registered()
		{
			MessageTypeDispatcher messageDispatcher = new MessageTypeDispatcher();
			SubscriptionCoordinator coordinator = new SubscriptionCoordinator(messageDispatcher, null, null, obj);

			CorrelatedController controller = new CorrelatedController(messageDispatcher, coordinator);

			controller.DoWork();

			Assert.That(controller.ResponseReceived, Is.True);
		}

		[Test]
		public void A_correlated_subscriber_should_not_register_a_general_subscription()
		{
			Uri uri = new Uri("msmq://localhost/test_servicebus");

			ISubscriptionCache cache = StrictMock<ISubscriptionCache>();
			IEndpoint endpoint = DynamicMock<IEndpoint>();
			IServiceBus bus = DynamicMock<IServiceBus>();
			SetupResult.For(bus.Endpoint).Return(endpoint);
			SetupResult.For(endpoint.Uri).Return(uri);

			MessageTypeDispatcher messageDispatcher = new MessageTypeDispatcher();
			SubscriptionCoordinator coordinator = new SubscriptionCoordinator(messageDispatcher, bus, cache, obj);
			CorrelatedController controller = new CorrelatedController(messageDispatcher, coordinator);

			using(Record())
			{
				cache.Add(new Subscription(typeof(ResponseMessage).FullName, controller.CorrelationId.ToString(), uri));
			}

			using (Playback())
			{
				controller.DoWork();

				Assert.That(controller.ResponseReceived, Is.True);
			}
		}
	}

	internal class CorrelatedController :
		Consumes<ResponseMessage>.For<Guid>
	{
		private readonly MessageTypeDispatcher _messageDispatcher;
		private readonly SubscriptionCoordinator _coordinator;
		private RequestMessage _request = new RequestMessage();

		private bool _responseReceived = false;

		public CorrelatedController(MessageTypeDispatcher messageDispatcher, SubscriptionCoordinator coordinator)
		{
			_messageDispatcher = messageDispatcher;
			_coordinator = coordinator;
		}

		public bool ResponseReceived
		{
			get { return _responseReceived; }
		}

		public Guid CorrelationId
		{
			get { return _request.CorrelationId; }
		}

		public void Consume(ResponseMessage message)
		{
			_responseReceived = true;
		}

		public void DoWork()
		{

			_coordinator.Resolve(this).Subscribe(this);

			ResponseMessage response = new ResponseMessage(_request.CorrelationId);

			_messageDispatcher.Consume(response);
		}
	}

	public class RequestMessage : CorrelatedBy<Guid>
	{
		private readonly Guid _correlationId = Guid.NewGuid();

		public Guid CorrelationId
		{
			get { return _correlationId; }
		}
	}

	public class ResponseMessage : CorrelatedBy<Guid>
	{
		private readonly Guid _correlationId;

        //xml serializer
        public ResponseMessage()
        {
        }

	    public ResponseMessage(Guid correlationId)
		{
			_correlationId = correlationId;
		}

		public Guid CorrelationId
		{
			get { return _correlationId; }
		}
	}
}