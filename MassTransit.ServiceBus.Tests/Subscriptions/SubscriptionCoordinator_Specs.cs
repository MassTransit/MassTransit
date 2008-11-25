namespace MassTransit.Tests.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Rhino.Mocks;
    using ServiceBus;
    using ServiceBus.Internal;
    using ServiceBus.Subscriptions;

    [TestFixture]
    public class When_the_SubscriptionCoordinator_is_interacting_with_the_SubscriptionCache :
        Specification
    {
        private IObjectBuilder _builder;
        private ISubscriptionCache _cache;
        private IServiceBus _bus;
        private IEndpoint _endpoint;
        private MessageTypeDispatcher _dispatcher;
        private TypeInfoCache _typeInfoCache;
        private DispatcherContext _context;

        protected override void Before_each()
        {
            _builder = DynamicMock<IObjectBuilder>();
            _bus = DynamicMock<IServiceBus>();
            _endpoint = DynamicMock<IEndpoint>();

            _cache = new LocalSubscriptionCache();

            SetupResult.For(_endpoint.Uri).Return(new Uri("msmq://localhost/test_queue"));
            SetupResult.For(_bus.Endpoint).Return(_endpoint);

            ReplayAll();

            _dispatcher = new MessageTypeDispatcher();

            _typeInfoCache = new TypeInfoCache();

            _context = new DispatcherContext(_builder, _bus, _dispatcher, _cache, _typeInfoCache);
        }

        [Test]
        public void Removing_a_message_type_subscription_should_not_leave_an_entry_in_the_cache()
        {
            AddAndRemoveSubscription<Consumer>();
        }

        [Test]
        public void Removing_a_correlated_subscription_should_not_leave_an_entry_in_the_cache()
        {
            AddAndRemoveSubscription<CorrelatedConsumer>();
        }

        protected void AddAndRemoveSubscription<T>() where T : class, new()
        {
            ISubscriptionTypeInfo info = _typeInfoCache.GetSubscriptionTypeInfo<T>();

            T c = new T();

            info.Subscribe(_context, c);

            IList<Subscription> entries = _cache.List();

            Assert.That(entries.Count, Is.EqualTo(1));

            info.Unsubscribe(_context, c);

            entries = _cache.List();

            Assert.That(entries.Count, Is.EqualTo(0));
        }

        [Test]
        public void Adding_two_message_type_consumers_and_removing_one_should_leave_one()
        {
            AddTwoAndRemoveOneSubscription<Consumer>();
        }

        [Test]
        public void Adding_two_correlated_consumers_and_removing_one_should_leave_one()
        {
            AddTwoAndRemoveOneSubscription<CorrelatedConsumer>();
        }

        protected void AddTwoAndRemoveOneSubscription<T>() where T : class, new()
        {
            ISubscriptionTypeInfo info = _typeInfoCache.GetSubscriptionTypeInfo<T>();

            T c = new T();

            info.Subscribe(_context, c);

            T d = new T();

            info.Subscribe(_context, d);

            IList<Subscription> entries = _cache.List();

            Assert.That(entries.Count, Is.EqualTo(1));

            info.Unsubscribe(_context, c);

            entries = _cache.List();

            Assert.That(entries.Count, Is.EqualTo(1));
        }

        internal class Consumer : Consumes<ExplicitMessage>.All
        {
            public void Consume(ExplicitMessage message)
            {
				
            }
        }

        internal class ExplicitMessage
        {
        }

        internal class CorrelatedConsumer : Consumes<CorrelatedMessage>.For<int>
        {
            public void Consume(CorrelatedMessage message)
            {
            }

            public int CorrelationId
            {
                get { return 27; }
            }
        }

        internal class CorrelatedMessage : CorrelatedBy<int>
        {
            public int CorrelationId
            {
                get { return 27; }
            }
        }
    }
}