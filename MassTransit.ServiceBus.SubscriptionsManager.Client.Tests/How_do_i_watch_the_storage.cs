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
        private ISubscriptionStorage mockCache;
        private IServiceBus mockBus;
        private IMessageQueueEndpoint mockEndpoint;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            mockCache = mocks.CreateMock<ISubscriptionStorage>();
            mockBus = mocks.CreateMock<IServiceBus>();
            mockEndpoint = mocks.CreateMock<IMessageQueueEndpoint>();
        }

        [TearDown]
        public void TearDown()
        {
            mocks = null;
            mockCache = null;
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

                mockCache.SubscriptionChanged += null;
                LastCall.IgnoreArguments();
                Expect.Call(mockCache.List()).Return(new List<Subscription>());
            }
            using(mocks.Playback())
            {
                ClientProxy proxy = new ClientProxy(mockEndpoint);
                proxy.StartWatching(mockBus, mockCache);    
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
                Expect.Call(delegate { mockCache.SubscriptionChanged += null; }).IgnoreArguments();
                eventRaiser = LastCall.GetEventRaiser();
                Expect.Call(mockCache.List()).Return(new List<Subscription>());

                Expect.Call(delegate { mockBus.Send<SubscriptionChange>(mockEndpoint, null); }).IgnoreArguments();

            }
            using (mocks.Playback())
            {
                ClientProxy proxy = new ClientProxy(mockEndpoint);
                proxy.StartWatching(mockBus, mockCache);

                eventRaiser.Raise(mockCache, new SubscriptionChangedEventArgs(new SubscriptionChange("bob", new Uri("msmq://localhost/bob"), SubscriptionChangeType.Add)));
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
                Expect.Call(delegate { mockCache.SubscriptionChanged += null; }).IgnoreArguments();
                
                eventRaiser = LastCall.GetEventRaiser();

                Expect.Call(delegate { mockBus.Send(null, new SubscriptionChange("",null,SubscriptionChangeType.Add)); }).IgnoreArguments();
                Expect.Call(mockCache.List()).Return(new List<Subscription>());
            }
            using (mocks.Playback())
            {
                ClientProxy proxy = new ClientProxy(mockEndpoint);
                proxy.StartWatching(mockBus, mockCache);

                eventRaiser.Raise(mockCache, new SubscriptionChangedEventArgs(new SubscriptionChange("bob", new Uri("msmq://localhost/bob"), SubscriptionChangeType.Add)));
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
                Expect.Call(delegate { mockCache.SubscriptionChanged += null; }).IgnoreArguments();

                eventRaiser = LastCall.GetEventRaiser();

                Expect.Call(delegate { mockBus.Send(null, new SubscriptionChange("", null, SubscriptionChangeType.Add)); }).IgnoreArguments();
                Expect.Call(mockCache.List()).Return(new List<Subscription>(new Subscription[] { new Subscription(new Uri("msmq://localhost/test"), "bob" )}));
                Expect.Call(delegate { mockBus.Send(null, new SubscriptionChange("", null, SubscriptionChangeType.Add)); }).IgnoreArguments();
            }
            using (mocks.Playback())
            {
                
                ClientProxy proxy = new ClientProxy(mockEndpoint);
                proxy.StartWatching(mockBus, mockCache);

                eventRaiser.Raise(mockCache, new SubscriptionChangedEventArgs(new SubscriptionChange("bob", new Uri("msmq://localhost/bob"), SubscriptionChangeType.Add)));
            }
        }


        [Test]
        public void When_we_get_notified_of_a_change_add()
        {
            List<SubscriptionChange> changes = new List<SubscriptionChange>();
            SubscriptionChange change = new SubscriptionChange(typeof(RequestCacheUpdate).FullName, new Uri("msmq://localhost/test"), SubscriptionChangeType.Add );
            changes.Add(change);

            List<CacheUpdateResponse> msgs = new List<CacheUpdateResponse>();
            CacheUpdateResponse msg = new CacheUpdateResponse(changes);
            msgs.Add(msg);
            

            using(mocks.Record())
            {
                Expect.Call(delegate { mockBus.Subscribe<CacheUpdateResponse>(null); }).IgnoreArguments();
                Expect.Call(delegate { mockBus.Send(null, new RequestCacheUpdate()); }).IgnoreArguments();
                Expect.Call(delegate { mockCache.SubscriptionChanged += null; }).IgnoreArguments();
                Expect.Call(mockCache.List()).Return(new List<Subscription>());

                //New Stuff
                Expect.Call(delegate { mockCache.Add(typeof (RequestCacheUpdate).FullName, new Uri("msmq://localhost/test")); });
                //TODO: does an event get fired?
            }
            using(mocks.Playback())
            {
                ClientProxy proxy = new ClientProxy(mockEndpoint);
                proxy.StartWatching(mockBus, mockCache);

                proxy.RespondToCacheUpdateMessage(new MessageContext<CacheUpdateResponse>(mockBus, new Envelope(msgs.ToArray()), msg));
            }
        }

        [Test]
        public void When_we_add_an_existing_subscription()
        {
            
        }

        [Test]
        public void When_we_get_notified_of_a_change_remove()
        {
            List<SubscriptionChange> changes = new List<SubscriptionChange>();
            SubscriptionChange change = new SubscriptionChange(typeof(RequestCacheUpdate).FullName, new Uri("msmq://localhost/test"), SubscriptionChangeType.Remove);
            changes.Add(change);

            List<CacheUpdateResponse> msgs = new List<CacheUpdateResponse>();
            CacheUpdateResponse msg = new CacheUpdateResponse(changes);
            msgs.Add(msg);


            using (mocks.Record())
            {
                Expect.Call(delegate { mockBus.Subscribe<CacheUpdateResponse>(null); }).IgnoreArguments();
                Expect.Call(delegate { mockBus.Send(null, new RequestCacheUpdate()); }).IgnoreArguments();
                Expect.Call(delegate { mockCache.SubscriptionChanged += null; }).IgnoreArguments();
                Expect.Call(mockCache.List()).Return(new List<Subscription>());

                //New Stuff
                Expect.Call(delegate { mockCache.Remove(typeof(RequestCacheUpdate).FullName, new Uri("msmq://localhost/test")); });
                //TODO: does an event get fired?
            }
            using (mocks.Playback())
            {
                ClientProxy proxy = new ClientProxy(mockEndpoint);
                proxy.StartWatching(mockBus, mockCache);

                proxy.RespondToCacheUpdateMessage(new MessageContext<CacheUpdateResponse>(mockBus, new Envelope(msgs.ToArray()), msg));
            }
        }


    }
}
