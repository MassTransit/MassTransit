namespace MassTransit.ServiceBus.Tests
{
    using System;
    using System.Collections;
    using MassTransit.ServiceBus.Internal;
    using MassTransit.ServiceBus.Subscriptions;
    using Messages;
    using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Rhino.Mocks;
    using Transports;

    [TestFixture]
    public class When_a_message_is_delivered_to_the_service_bus :
        Specification
    {

        private ServiceBus _bus;
        private IObjectBuilder obj;
        private EndpointResolver _endpointResolver;
        private IEndpoint _endpoint;
        private Uri _endpointUri = new Uri("loopback://localhost/test");

        protected override void Before_each()
        {
            _endpointResolver = new EndpointResolver();
            EndpointResolver.AddTransport(typeof(LoopbackEndpoint));

            _endpoint = _endpointResolver.Resolve(_endpointUri);

            obj = DynamicMock<IObjectBuilder>();

            _bus = new ServiceBus(_endpoint, obj, new LocalSubscriptionCache(), _endpointResolver, new TypeInfoCache());
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

            SetupResult.For(obj.GetInstance<PingHandler>()).Return(ph);
            SetupResult.For(obj.GetInstance<PingHandler>(new Hashtable())).IgnoreArguments().Return(ph);

            ReplayAll();

            _bus.Subscribe<PingHandler>();

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