namespace MassTransit.ServiceBus.Tests.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.ServiceBus.Subscriptions.Messages;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class As_a_SubscriptionService
    {
        private MockRepository mocks;
        private IServiceBus mockBus;
        private ISubscriptionCache cache;
        private ISubscriptionRepository mockRepository;
        private SubscriptionService srv;

        private IMessageContext<AddSubscription> msgCxtAdd;
        private IMessageContext<RemoveSubscription> msgCxtRem;
        private AddSubscription msgAdd;
        private RemoveSubscription msgRem;

        [SetUp]
        public void I_want_to()
        {
            mocks = new MockRepository();
            mockBus = mocks.CreateMock<IServiceBus>();
            mockRepository = mocks.CreateMock<ISubscriptionRepository>();

            msgAdd = new AddSubscription("bob", new Uri("queue:\\bob"));
            msgCxtAdd = mocks.CreateMock<IMessageContext<AddSubscription>>();
            msgRem = new RemoveSubscription("bob", new Uri("queue:\\bob"));
            msgCxtRem = mocks.CreateMock<IMessageContext<RemoveSubscription>>();

            SetupResult.For(msgCxtAdd.Message).Return(msgAdd);
            SetupResult.For(msgCxtRem.Message).Return(msgRem);

            cache = new LocalSubscriptionCache();
            srv = new SubscriptionService(mockBus, cache, mockRepository);
        }


        [Test]
        public void be_alive()
        {
            Assert.IsNotNull(srv);
        }

        [Test]
        public void be_startable()
        {
            IEnumerable<Subscription> enumer = new List<Subscription>();

            using(mocks.Record())
            {
                Expect.Call(mockRepository.List()).Return(enumer);
                Expect.Call(delegate { mockBus.Subscribe<CacheUpdateRequest>(null); }).IgnoreArguments();
                Expect.Call(delegate { mockBus.Subscribe<AddSubscription>(null); }).IgnoreArguments();
                Expect.Call(delegate { mockBus.Subscribe<RemoveSubscription>(null); }).IgnoreArguments();
                Expect.Call(delegate { mockBus.Subscribe<CancelSubscriptionUpdates>(null); }).IgnoreArguments();
            }
            using(mocks.Playback())
            {
                srv.Start();
            }
        }

        [Test]
        public void be_stopable()
        {
            using (mocks.Record())
            {
                Expect.Call(delegate { mockBus.Unsubscribe<CacheUpdateRequest>(null); }).IgnoreArguments();
                Expect.Call(delegate { mockBus.Unsubscribe<AddSubscription>(null); }).IgnoreArguments();
                Expect.Call(delegate { mockBus.Unsubscribe<RemoveSubscription>(null); }).IgnoreArguments();
                Expect.Call(delegate { mockBus.Unsubscribe<CancelSubscriptionUpdates>(null); }).IgnoreArguments();

            }
            using (mocks.Playback())
            {
                srv.Stop();
            }
        }

        [Test]
        public void add_subscriptions_from_messages()
        {
            using(mocks.Record())
            {
                Expect.Call(delegate
                                {
                                    mockRepository.Save(msgAdd.Subscription);
                                });
            }
            using (mocks.Playback())
            {
                srv.HandleAddSubscription(msgCxtAdd);
            }
        }

        [Test]
        public void remove_subscriptions_from_messages()
        {
            using (mocks.Record())
            {
                Expect.Call(delegate
                                {
                                    mockRepository.Remove(msgRem.Subscription);
                                });
            }
            using (mocks.Playback())
            {
                srv.HandleRemoveSubscription(msgCxtRem);
            }
        }
    }
}