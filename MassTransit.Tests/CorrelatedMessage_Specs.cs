namespace MassTransit.Tests
{
    using System;
    using MassTransit.Internal;
    using MassTransit.Subscriptions;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class When_a_correlated_message_is_received :
        Specification
    {
        private DispatcherContext _context;
        private ISubscriptionCache cache;
        private Uri uri;

        protected override void Before_each()
        {
            uri = new Uri("msmq://localhost/test_servicebus");

            cache = DynamicMock<ISubscriptionCache>();
            IEndpoint endpoint = DynamicMock<IEndpoint>();
            IServiceBus bus = DynamicMock<IServiceBus>();
            SetupResult.For(bus.Endpoint).Return(endpoint);
            SetupResult.For(endpoint.Uri).Return(uri);

            MessageTypeDispatcher messageDispatcher = new MessageTypeDispatcher();
            TypeInfoCache typeInfoCache = new TypeInfoCache();

            IObjectBuilder builder = DynamicMock<IObjectBuilder>();

            _context = new DispatcherContext(builder, bus, messageDispatcher, cache, typeInfoCache);
        }

        [Test]
        public void A_type_should_be_registered()
        {
            ReplayAll();

            CorrelatedController controller = new CorrelatedController(_context);

            controller.DoWork();

            Assert.That(controller.ResponseReceived, Is.True);
        }

        [Test]
        public void A_correlated_subscriber_should_not_register_a_general_subscription()
        {
            CorrelatedController controller = new CorrelatedController(_context);

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
        private readonly RequestMessage _request = new RequestMessage();

        private bool _responseReceived;
        private readonly IDispatcherContext _context;

        public CorrelatedController(IDispatcherContext context)
        {
            _context = context;
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
            _context.GetSubscriptionTypeInfo(GetType()).Subscribe(_context, this);

            ResponseMessage response = new ResponseMessage(_request.CorrelationId);

            _context.Consume(response);
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