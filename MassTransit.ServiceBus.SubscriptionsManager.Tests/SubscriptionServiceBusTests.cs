namespace MassTransit.ServiceBus.SubscriptionsManager.Tests
{
    using System;
    using System.Collections.Generic;
    using MassTransit.ServiceBus.Subscriptions.Messages;
    using NUnit.Framework;
    using Rhino.Mocks;
    using Subscriptions;

    [TestFixture]
    public class When_a_CacheUpdateRequest_is_recieved
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            _mocks = new MockRepository();
            _repository = _mocks.CreateMock<ISubscriptionStorage>();
            _bus = _mocks.CreateMock<IServiceBus>();
            _cache = _mocks.CreateMock<ISubscriptionStorage>();

            _service = new SubscriptionService(_bus, _cache, _repository);

            _context = _mocks.CreateMock<IMessageContext<CacheUpdateRequest>>();
        }

        #endregion

        private MockRepository _mocks;
        private ISubscriptionStorage _repository;
        private ISubscriptionStorage _cache;
        private IServiceBus _bus;
        private SubscriptionService _service;
        private IMessageContext<CacheUpdateRequest> _context;

        [Test]
        public void Reply_with_a_list_of_subscriptions()
        {
            List<Subscription> subscriptions = new List<Subscription>();
            subscriptions.Add(new Subscription(new Uri("msmq://localhost/test"), "a"));

            CacheUpdateResponse response = new CacheUpdateResponse(subscriptions);

            using (_mocks.Record())
            {
                Expect.Call(_cache.List()).Return(subscriptions);
                _context.Reply(response);
                LastCall.IgnoreArguments();
            }

            using (_mocks.Playback())
            {
                _service.HandleCacheUpdateRequest(_context);
            }
        }
    }

    [TestFixture]
    public class When_a_SubscriptionChange_message_is_received_by_the_SubscriptionService
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            _mocks = new MockRepository();
            _repository = _mocks.CreateMock<ISubscriptionStorage>();
            _bus = _mocks.CreateMock<IServiceBus>();
            _cache = _mocks.CreateMock<ISubscriptionStorage>();

            _service = new SubscriptionService(_bus, _cache, _repository);

            _context = _mocks.CreateMock<IMessageContext<SubscriptionChange>>();

            _uri = new Uri("msmq://localhost/test");
        }

        #endregion

        private MockRepository _mocks;
        private ISubscriptionStorage _repository;
        private ISubscriptionStorage _cache;
        private IServiceBus _bus;
        private SubscriptionService _service;
        private IMessageContext<SubscriptionChange> _context;
        private SubscriptionChange _message;
        private Uri _uri;

        [Test]
        public void If_the_message_is_a_remove_update_the_repository_and_service_bus()
        {
            _message = new SubscriptionChange("a", _uri, SubscriptionChangeType.Remove);

            using (_mocks.Record())
            {
                Expect.Call(_context.Message).Return(_message).Repeat.AtLeastOnce();
                Expect.Call(delegate { _cache.Remove("a", _uri); });
                Expect.Call(delegate { _repository.Remove("a", _uri); });
            }

            using (_mocks.Playback())
            {
                _service.HandleSubscriptionChange(_context);
            }
        }

        [Test]
        public void If_the_message_is_an_add_update_the_repository_and_service_bus()
        {
            _message = new SubscriptionChange("a", _uri, SubscriptionChangeType.Add);

            using (_mocks.Record())
            {
                Expect.Call(_context.Message).Return(_message).Repeat.AtLeastOnce();
                Expect.Call(delegate { _cache.Add("a", _uri); });
                Expect.Call(delegate { _repository.Add("a", _uri); });
            }

            using (_mocks.Playback())
            {
                _service.HandleSubscriptionChange(_context);
            }
        }
    }
}