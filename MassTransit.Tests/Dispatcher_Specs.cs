namespace MassTransit.Tests
{
    using System;
    using System.Collections;
    using Configuration;
    using MassTransit.Internal;
    using MassTransit.Serialization;
    using MassTransit.Subscriptions;
    using MassTransit.Transports;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;
    
    using Tests.Messages;

    [TestFixture]
    public class When_a_message_is_delivered_to_the_service_bus :
        Specification
    {

        private ServiceBus _bus;
        private IObjectBuilder _builder;
        private IEndpointFactory _endpointFactory;
        private IEndpoint _endpoint;
        private Uri _endpointUri = new Uri("loopback://localhost/test");

        protected override void Before_each()
        {
			_builder = MockRepository.GenerateMock<IObjectBuilder>();
			_builder.Stub(x => x.GetInstance<BinaryMessageSerializer>()).Return(new BinaryMessageSerializer());

			_endpointFactory = EndpointFactoryConfigurator.New(x =>
			{
				x.SetObjectBuilder(_builder);
				x.SetDefaultSerializer<BinaryMessageSerializer>();
				x.RegisterTransport<LoopbackEndpoint>();
			});

            _endpoint = _endpointFactory.GetEndpoint(_endpointUri);


            _bus = new ServiceBus(_endpoint, _builder, new LocalSubscriptionCache(), _endpointFactory, new TypeInfoCache());
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

        	_builder.Stub(x => x.GetInstance<PingHandler>()).Return(ph);
			_builder.Stub(x => x.GetInstance<PingHandler>(new Hashtable())).IgnoreArguments().Return(ph);

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