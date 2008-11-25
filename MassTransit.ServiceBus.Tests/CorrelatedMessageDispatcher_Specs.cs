namespace MassTransit.Tests
{
    using System;
    using System.Diagnostics;
    using MassTransit.Internal;
    using MassTransit.Subscriptions;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;

    [TestFixture]
    public class When_a_correlated_message_is_dispatched_through_the_message_dispatcher :
        Specification
    {
        private IObjectBuilder obj;
        private MessageTypeDispatcher _dispatcher;
        private TestMessage _message;
        private const int _value = 27;
        private TypeInfoCache _typeInfoCache;
        private DispatcherContext _context;
        private IServiceBus _bus;
        private ISubscriptionCache _cache;
        private IEndpoint _endpoint;

        protected override void Before_each()
        {
            obj = StrictMock<IObjectBuilder>();
            _dispatcher = new MessageTypeDispatcher();
            _typeInfoCache = new TypeInfoCache();
            _message = new TestMessage(_value);

            _endpoint = DynamicMock<IEndpoint>();
            SetupResult.For(_endpoint.Uri).Return(new Uri("msmq://localhost/queue"));

            _bus = DynamicMock<IServiceBus>();
            SetupResult.For(_bus.Endpoint).Return(_endpoint);

            _cache = new LocalSubscriptionCache();

            _context = new DispatcherContext(obj, _bus, _dispatcher, _cache, _typeInfoCache);

            ReplayAll();

        }

        [Test]
        public void It_should_be_dispatched_to_all_consumers()
        {
            TestConsumer consumerA = new TestConsumer(_message.CorrelationId);
            _typeInfoCache.GetSubscriptionTypeInfo(consumerA.GetType()).Subscribe(_context, consumerA);

            TestConsumer consumerB = new TestConsumer(_message.CorrelationId);
            _typeInfoCache.GetSubscriptionTypeInfo(consumerB.GetType()).Subscribe(_context, consumerB);

            _dispatcher.Consume(_message);

            Assert.That(consumerA.Value, Is.EqualTo(_value));
            Assert.That(consumerB.Value, Is.EqualTo(_value));
        }

        [Test, Explicit]
        public void Verify_the_throughput_of_the_dispatcher()
        {
            TestConsumer consumerA = new TestConsumer(_message.CorrelationId);
            _typeInfoCache.GetSubscriptionTypeInfo(consumerA.GetType()).Subscribe(_context, consumerA);

            TestConsumer consumerB = new TestConsumer(Guid.NewGuid());
            _typeInfoCache.GetSubscriptionTypeInfo(consumerB.GetType()).Subscribe(_context, consumerB);

            const long limit = 5000000;

            DateTime start = DateTime.Now;

            for (long i = 0; i < limit; i++)
            {
                _dispatcher.Consume(_message);
            }

            DateTime stop = DateTime.Now;

            Debug.WriteLine(string.Format("Messages per second dispatched: {0}", limit/(stop - start).TotalMilliseconds*1000));

            Assert.That(consumerA.Value, Is.EqualTo(_value));
            Assert.That(consumerB.Value, Is.EqualTo(_value));
        }

        [Test]
        public void It_should_be_dispatched_to_the_consumer()
        {
            TestConsumer consumerA = new TestConsumer(_message.CorrelationId);
            _typeInfoCache.GetSubscriptionTypeInfo(consumerA.GetType()).Subscribe(_context, consumerA);

            _dispatcher.Consume(_message);

            Assert.That(consumerA.Value, Is.EqualTo(_message.Value));
        }

        [Test]
        public void It_should_be_dispatched_to_the_consumer_without_issues()
        {
            TestConsumer consumerA = new TestConsumer(_message.CorrelationId);
            _typeInfoCache.GetSubscriptionTypeInfo(consumerA.GetType()).Subscribe(_context, consumerA);

            TestConsumer consumerB = new TestConsumer(Guid.NewGuid());
            _typeInfoCache.GetSubscriptionTypeInfo(consumerB.GetType()).Subscribe(_context, consumerB);

            _dispatcher.Consume(_message);

            Assert.That(consumerA.Value, Is.EqualTo(_message.Value));
        }

        [Test]
        public void It_should_be_sent_to_general_consumers_who_are_not_correlated_consumers()
        {
            GeneralConsumer consumerA = new GeneralConsumer();
            _typeInfoCache.GetSubscriptionTypeInfo(consumerA.GetType()).Subscribe(_context, consumerA);

            _dispatcher.Consume(_message);

            Assert.That(consumerA.Value, Is.EqualTo(_message.Value));
        }

        [Test]
        public void It_should_be_sent_to_more_than_one_general_consumer_who_are_not_correlated_consumers()
        {
            GeneralConsumer consumerA = new GeneralConsumer();
            _typeInfoCache.GetSubscriptionTypeInfo(consumerA.GetType()).Subscribe(_context, consumerA);
            GeneralConsumer consumerB = new GeneralConsumer();
            _typeInfoCache.GetSubscriptionTypeInfo(consumerB.GetType()).Subscribe(_context, consumerB);

            _dispatcher.Consume(_message);

            Assert.That(consumerA.Value, Is.EqualTo(_message.Value));
            Assert.That(consumerB.Value, Is.EqualTo(_message.Value));
        }

        [Test]
        public void It_should_not_be_dispatched_if_the_correlation_does_not_match()
        {
            TestConsumer consumerA = new TestConsumer(Guid.NewGuid());
            _typeInfoCache.GetSubscriptionTypeInfo(consumerA.GetType()).Subscribe(_context, consumerA);

            _dispatcher.Consume(_message);

            Assert.That(consumerA.Value, Is.EqualTo(default(int)));
        }

        [Test]
        public void It_should_not_be_sent_to_consumers_that_are_no_longer_interested()
        {
            TestConsumer consumerA = new TestConsumer(_message.CorrelationId);
            _typeInfoCache.GetSubscriptionTypeInfo(consumerA.GetType()).Subscribe(_context, consumerA);

            TestConsumer consumerB = new TestConsumer(Guid.NewGuid());
            _typeInfoCache.GetSubscriptionTypeInfo(consumerB.GetType()).Subscribe(_context, consumerB);

            _dispatcher.Consume(_message);

            Assert.That(consumerA.Value, Is.EqualTo(_value));
            Assert.That(consumerB.Value, Is.EqualTo(default(int)));
        }

        [Test]
        public void It_should_not_be_sent_to_uninterested_consumers()
        {
            TestConsumer consumerA = new TestConsumer(_message.CorrelationId);
            _typeInfoCache.GetSubscriptionTypeInfo(consumerA.GetType()).Subscribe(_context, consumerA);

            TestConsumer consumerB = new TestConsumer(_message.CorrelationId);
            _typeInfoCache.GetSubscriptionTypeInfo(consumerB.GetType()).Subscribe(_context, consumerB);

            _typeInfoCache.GetSubscriptionTypeInfo(consumerA.GetType()).Unsubscribe(_context, consumerA);

            _dispatcher.Consume(_message);

            Assert.That(consumerA.Value, Is.EqualTo(default(int)));
            Assert.That(consumerB.Value, Is.EqualTo(_value));
        }

        [Test]
        public void It_should_only_be_sent_to_consumers_who_are_interested_in_the_specific_correlation_id()
        {
            TestMessage anotherMessage = new TestMessage(42);

            TestConsumer consumerA = new TestConsumer(_message.CorrelationId);
            _typeInfoCache.GetSubscriptionTypeInfo(consumerA.GetType()).Subscribe(_context, consumerA);

            TestConsumer consumerB = new TestConsumer(anotherMessage.CorrelationId);
            _typeInfoCache.GetSubscriptionTypeInfo(consumerB.GetType()).Subscribe(_context, consumerB);

            _dispatcher.Consume(_message);
            _dispatcher.Consume(anotherMessage);

            Assert.That(consumerA.Value, Is.EqualTo(_value));
            Assert.That(consumerB.Value, Is.EqualTo(42));
        }

        [Test]
        public void The_object_should_be_dispatched_to_the_consumer()
        {
            TestConsumer consumerA = new TestConsumer(_message.CorrelationId);
            _typeInfoCache.GetSubscriptionTypeInfo(consumerA.GetType()).Subscribe(_context, consumerA);

            object message = _message;

            _dispatcher.Consume(message);

            Assert.That(consumerA.Value, Is.EqualTo(_message.Value));
        }


        internal class InvalidConsumer
        {
        }

        internal class TestConsumer : Consumes<TestMessage>.For<Guid>
        {
            private readonly Guid _correlationId;
            private int _value;

            public TestConsumer(Guid correlationId)
            {
                _correlationId = correlationId;
            }

            public int Value
            {
                get { return _value; }
            }

            public Guid CorrelationId
            {
                get { return _correlationId; }
            }

            public void Consume(TestMessage message)
            {
                _value = message.Value;
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
    }
}