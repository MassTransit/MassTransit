namespace MassTransit.Tests
{
    using System;
    using System.Threading;
    using Configuration;
    using MassTransit.Internal;
    using MassTransit.Serialization;
    using MassTransit.Subscriptions;
    using MassTransit.Transports;
    using NUnit.Framework;
    using Rhino.Mocks;

	[TestFixture]
    public class When_a_message_fault_occurs :
        Specification
    {
        [Test]
        public void I_should_receive_a_fault_message()
        {
            SmartConsumer sc = new SmartConsumer();

            _bus.Subscribe<Hello>(delegate { throw new AccessViolationException("Crap!"); });

            _bus.Subscribe(sc);

            _bus.Publish(new Hello());

            Assert.IsTrue(sc.GotFault.WaitOne(TimeSpan.FromSeconds(5), true));
        }

        private LocalSubscriptionCache _cache;
        private IEndpointFactory _resolver;
        private IEndpoint _endpoint;
        private ServiceBus _bus;
        private IObjectBuilder _builder;

        protected override void Before_each()
        {
			_cache = new LocalSubscriptionCache();
			_builder = MockRepository.GenerateMock<IObjectBuilder>();
			_resolver = EndpointFactoryConfigurator.New(x =>
			{
				x.SetObjectBuilder(_builder);
				x.SetDefaultSerializer<BinaryMessageSerializer>();
				x.RegisterTransport<LoopbackEndpoint>();
			});
			_endpoint = _resolver.GetEndpoint(new Uri("loopback://localhost/servicebus"));
			_bus = new ServiceBus(_endpoint, _builder, _cache, _resolver, new TypeInfoCache());
			_bus.Start();
		}

        protected override void After_each()
        {
            _bus.Dispose();
            _endpoint.Dispose();
        	_resolver.Dispose();
            _cache.Dispose();
        }

        public class SmartConsumer :
            Consumes<Fault<Hello>>.All
        {
            private readonly ManualResetEvent _gotFault = new ManualResetEvent(false);

            public ManualResetEvent GotFault
            {
                get { return _gotFault; }
            }

            public void Consume(Fault<Hello> message)
            {
                _gotFault.Set();
            }
        }

        [Serializable]
        public class Hello
        {
        }

        [Serializable]
        public class Hi
        {
        }
    }

    [TestFixture]
    public class When_a_correlated_message_fault_is_received :
        Specification
    {
        [Test]
        public void I_should_receive_a_fault_message()
        {
            SmartConsumer sc = new SmartConsumer();

            _bus.Subscribe<Hello>(delegate { throw new AccessViolationException("Crap!"); });

            _bus.Subscribe(sc);

            _bus.Publish(new Hello(sc.CorrelationId));

            Assert.IsTrue(sc.GotFault.WaitOne(TimeSpan.FromSeconds(5), true));
        }

        private LocalSubscriptionCache _cache;
        private IEndpointFactory _resolver;
        private IEndpoint _endpoint;
        private ServiceBus _bus;
        private IObjectBuilder _builder;

        protected override void Before_each()
        {
			_cache = new LocalSubscriptionCache();
			_builder = MockRepository.GenerateMock<IObjectBuilder>();
			_resolver = EndpointFactoryConfigurator.New(x =>
			{
				x.SetObjectBuilder(_builder);
				x.SetDefaultSerializer<BinaryMessageSerializer>();
				x.RegisterTransport<LoopbackEndpoint>();
			});
			_endpoint = _resolver.GetEndpoint(new Uri("loopback://localhost/servicebus"));
			_bus = new ServiceBus(_endpoint, _builder, _cache, _resolver, new TypeInfoCache());
			_bus.Start();
		}


        protected override void After_each()
        {
			_bus.Dispose();
			_endpoint.Dispose();
			_resolver.Dispose();
			_cache.Dispose();
		}

        public class SmartConsumer :
            Consumes<Fault<Hello, Guid>>.For<Guid>
        {
            private readonly ManualResetEvent _gotFault = new ManualResetEvent(false);
            private readonly Guid _id = Guid.NewGuid();

            public ManualResetEvent GotFault
            {
                get { return _gotFault; }
            }

            public void Consume(Fault<Hello, Guid> message)
            {
                _gotFault.Set();
            }

            public Guid CorrelationId
            {
                get { return _id; }
            }
        }

        [Serializable]
        public class Hello : 
            CorrelatedBy<Guid>
        {
            private Guid _id;

            protected Hello()
            {
            }

            public Hello(Guid id)
            {
                _id = id;
            }

            public Guid CorrelationId
            {
                get { return _id; }
                set { _id = value; }
            }
        }

        [Serializable]
        public class Hi
        {
        }
    }
}