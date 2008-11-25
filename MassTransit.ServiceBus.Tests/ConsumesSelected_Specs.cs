namespace MassTransit.Tests
{
    using System;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;
    using ServiceBus;
    using ServiceBus.Internal;
    using ServiceBus.Subscriptions;

    [TestFixture]
    public class When_a_message_is_received_for_a_selective_consumer :
        Specification
    {
        private MessageTypeDispatcher _dispatcher;
        private TestMessage _message;
        private readonly int _value = 27;
        private IObjectBuilder _builder;
        private TypeInfoCache _typeInfoCache;
        private IDispatcherContext _context;
        private IServiceBus _bus;
        private ISubscriptionCache _cache;
        private IEndpoint _endpoint;

        protected override void Before_each()
        {
            _builder = StrictMock<IObjectBuilder>();

            _dispatcher = new MessageTypeDispatcher();
            _typeInfoCache = new TypeInfoCache();
            _message = new TestMessage(_value);

            _endpoint = DynamicMock<IEndpoint>();
            SetupResult.For(_endpoint.Uri).Return(new Uri("loopback://localhost/test"));

            _bus = DynamicMock<IServiceBus>();
            SetupResult.For(_bus.Endpoint).Return(_endpoint);

            _cache = new LocalSubscriptionCache();

            _context = new DispatcherContext(_builder, _bus, _dispatcher, _cache, _typeInfoCache);

            ReplayAll();
        }


        internal class InvalidConsumer
        {
        }

        internal class TestConsumer : Consumes<TestMessage>.Selected
        {
            private int _value;
            private readonly Predicate<TestMessage> _accept;

            public TestConsumer(Predicate<TestMessage> accept)
            {
                _accept = accept;
            }

            public int Value
            {
                get { return _value; }
            }

            public void Consume(TestMessage message)
            {
                _value = message.Value;
            }

            public bool Accept(TestMessage message)
            {
                return _accept(message);
            }
        }

        internal class TestMessage : CorrelatedBy<Guid>
        {
            private readonly Guid _correlationId;
            private readonly int _value;

            public TestMessage(int value)
            {
                _value = value;
                _correlationId = Guid.NewGuid();
            }

            public int Value
            {
                get { return _value; }
            }

            public Guid CorrelationId
            {
                get { return _correlationId; }
            }
        }

        internal class GeneralConsumer : Consumes<TestMessage>.All
        {
            private int _value;

            public int Value
            {
                get { return _value; }
            }

            public void Consume(TestMessage message)
            {
                _value = message.Value;
            }
        }

        [Test]
        public void It_should_only_be_dispatched_to_interested_consumers()
        {
            TestConsumer consumerA = new TestConsumer(delegate(TestMessage message) { return message.Value >= 32; });

            _typeInfoCache.GetSubscriptionTypeInfo(consumerA.GetType()).Subscribe(_context, consumerA);

            GeneralConsumer consumerB = new GeneralConsumer();
            _typeInfoCache.GetSubscriptionTypeInfo(consumerB.GetType()).Subscribe(_context, consumerB);

            _dispatcher.Consume(_message);

            Assert.That(consumerA.Value, Is.EqualTo(default(int)));
            Assert.That(consumerB.Value, Is.EqualTo(_value));
        }

        [Test]
        public void It_should_only_be_dispatched_to_interested_consumers_again()
        {
            TestConsumer consumerA = new TestConsumer(delegate(TestMessage message) { return message.Value >= 32; });
            _typeInfoCache.GetSubscriptionTypeInfo(consumerA.GetType()).Subscribe(_context, consumerA);

            TestConsumer consumerB = new TestConsumer(delegate(TestMessage message) { return message.Value < 32; });
            _typeInfoCache.GetSubscriptionTypeInfo(consumerB.GetType()).Subscribe(_context, consumerB);

            _dispatcher.Consume(_message);

            Assert.That(consumerA.Value, Is.EqualTo(default(int)));
            Assert.That(consumerB.Value, Is.EqualTo(_value));
        }
    }
}