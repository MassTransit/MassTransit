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
        private CacheUpdateRequest msgUpdate;
        private IMessageContext<CacheUpdateRequest> msgCxtUpdate;
        private CancelSubscriptionUpdates msgCancel;
        private IMessageContext<CancelSubscriptionUpdates> msgCxtCancel;

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
            msgUpdate = new CacheUpdateRequest();
            msgCxtUpdate = mocks.CreateMock<IMessageContext<CacheUpdateRequest>>();
            msgCancel = new CancelSubscriptionUpdates();
            msgCxtCancel = mocks.CreateMock<IMessageContext<CancelSubscriptionUpdates>>();

            SetupResult.For(msgCxtAdd.Message).Return(msgAdd);
            SetupResult.For(msgCxtRem.Message).Return(msgRem);
            SetupResult.For(msgCxtUpdate.Message).Return(msgUpdate);
            SetupResult.For(msgCxtCancel.Message).Return(msgCancel);


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
				Expect.Call(delegate { mockBus.Subscribe<CacheUpdateRequest>(delegate { }); }).IgnoreArguments();
				Expect.Call(delegate { mockBus.Subscribe<AddSubscription>(delegate { }); }).IgnoreArguments();
				Expect.Call(delegate { mockBus.Subscribe<RemoveSubscription>(delegate { }); }).IgnoreArguments();
				Expect.Call(delegate { mockBus.Subscribe<CancelSubscriptionUpdates>(delegate { }); }).IgnoreArguments();
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
				Expect.Call(delegate { mockBus.Unsubscribe<CacheUpdateRequest>(delegate { }); }).IgnoreArguments();
				Expect.Call(delegate { mockBus.Unsubscribe<AddSubscription>(delegate { }); }).IgnoreArguments();
				Expect.Call(delegate { mockBus.Unsubscribe<RemoveSubscription>(delegate { }); }).IgnoreArguments();
				Expect.Call(delegate { mockBus.Unsubscribe<CancelSubscriptionUpdates>(delegate { }); }).IgnoreArguments();

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

        [Test]
        public void respond_to_cache_updates()
        {
            using (mocks.Record())
            {
                Expect.Call(delegate { msgCxtUpdate.Reply(null); }).IgnoreArguments();
            }
            using (mocks.Playback())
            {
                srv.HandleCacheUpdateRequest(msgCxtUpdate);
            }
        }

        [Test]
        public void respond_to_update_cancel()
        {
            using (mocks.Record())
            {
                
            }
            using (mocks.Playback())
            {
                srv.HandleCancelSubscriptionUpdates(msgCxtCancel);
            }   
        }
    }
}