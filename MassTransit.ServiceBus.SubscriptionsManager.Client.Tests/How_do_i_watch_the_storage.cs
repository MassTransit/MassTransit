namespace MassTransit.ServiceBus.SubscriptionsManager.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using MassTransit.ServiceBus.Subscriptions.Messages;
    using NUnit.Framework;
    using Rhino.Mocks;
    using Rhino.Mocks.Interfaces;
    using Subscriptions;

    [TestFixture]
    public class How_do_i_watch_the_storage
    {
        MockRepository mocks;
        private ISubscriptionStorage cache;
        private IServiceBus mockBus;
        private IMessageQueueEndpoint mockEndpoint;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            cache = mocks.CreateMock<ISubscriptionStorage>();
            mockBus = mocks.CreateMock<IServiceBus>();
            mockEndpoint = mocks.CreateMock<IMessageQueueEndpoint>();
        }

        [TearDown]
        public void TearDown()
        {
            mocks = null;
            cache = null;
            mockBus = null;
            mockEndpoint = null;
        }

        [Test]
        public void When_first_watching()
        {
            using(mocks.Record())
            {
                mockBus.Subscribe<CacheUpdateResponse>(null);
                LastCall.IgnoreArguments();
                mockBus.Send<RequestCacheUpdate>(mockEndpoint, null);
                LastCall.IgnoreArguments();

                cache.SubscriptionChanged += null;
                LastCall.IgnoreArguments();
                Expect.Call(cache.List()).Return(new List<Subscription>());
            }
            using(mocks.Playback())
            {
                ClientProxy proxy = new ClientProxy(mockEndpoint);
                proxy.StartWatching(mockBus, cache);    
            }
        }

        [Test]
        public void When_responding_to_a_subscription_change()
        {
            IEventRaiser eventRaiser;
            using (mocks.Record())
            {
                mockBus.Subscribe<CacheUpdateResponse>(null);
                LastCall.IgnoreArguments();
                mockBus.Send<RequestCacheUpdate>(mockEndpoint, null);
                LastCall.IgnoreArguments();
                Expect.Call(delegate { cache.SubscriptionChanged += null; }).IgnoreArguments();
                eventRaiser = LastCall.GetEventRaiser();
                Expect.Call(cache.List()).Return(new List<Subscription>());

                Expect.Call(delegate { mockBus.Send<SubscriptionChange>(mockEndpoint, null); }).IgnoreArguments();

            }
            using (mocks.Playback())
            {
                ClientProxy proxy = new ClientProxy(mockEndpoint);
                proxy.StartWatching(mockBus, cache);

                eventRaiser.Raise(cache, new SubscriptionChangedEventArgs(new SubscriptionChange("bob", new Uri("msmq://localhost/bob"), SubscriptionChangeType.Add)));
            }
        }

        [Test]
        public void When_responding_to_a_subscription_change_bus_interaction()
        {
            IEventRaiser eventRaiser;
            using (mocks.Record())
            {
                Expect.Call(delegate { mockBus.Subscribe<CacheUpdateResponse>(null); }).IgnoreArguments();
                Expect.Call(delegate { mockBus.Send(null, new RequestCacheUpdate()); }).IgnoreArguments();
                Expect.Call(delegate { cache.SubscriptionChanged += null; }).IgnoreArguments();
                
                eventRaiser = LastCall.GetEventRaiser();

                Expect.Call(delegate { mockBus.Send(null, new SubscriptionChange("",null,SubscriptionChangeType.Add)); }).IgnoreArguments();
                Expect.Call(cache.List()).Return(new List<Subscription>());
            }
            using (mocks.Playback())
            {
                ClientProxy proxy = new ClientProxy(mockEndpoint);
                proxy.StartWatching(mockBus, cache);

                eventRaiser.Raise(cache, new SubscriptionChangedEventArgs(new SubscriptionChange("bob", new Uri("msmq://localhost/bob"), SubscriptionChangeType.Add)));
            }
        }

        [Test]
        public void Starting_to_watch_when_there_are_subscriptions_already()
        {
            IEventRaiser eventRaiser;
            using (mocks.Record())
            {
                Expect.Call(delegate { mockBus.Subscribe<CacheUpdateResponse>(null); }).IgnoreArguments();
                Expect.Call(delegate { mockBus.Send(null, new RequestCacheUpdate()); }).IgnoreArguments();
                Expect.Call(delegate { cache.SubscriptionChanged += null; }).IgnoreArguments();

                eventRaiser = LastCall.GetEventRaiser();

                Expect.Call(delegate { mockBus.Send(null, new SubscriptionChange("", null, SubscriptionChangeType.Add)); }).IgnoreArguments();
                Expect.Call(cache.List()).Return(new List<Subscription>(new Subscription[] { new Subscription(new Uri("msmq://localhost/test"), "bob" )}));
                Expect.Call(delegate { mockBus.Send(null, new SubscriptionChange("", null, SubscriptionChangeType.Add)); }).IgnoreArguments();
            }
            using (mocks.Playback())
            {
                
                ClientProxy proxy = new ClientProxy(mockEndpoint);
                proxy.StartWatching(mockBus, cache);

                eventRaiser.Raise(cache, new SubscriptionChangedEventArgs(new SubscriptionChange("bob", new Uri("msmq://localhost/bob"), SubscriptionChangeType.Add)));
            }
        }


    }
}
