namespace MassTransit.ServiceBus.SubscriptionsManager.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using MassTransit.ServiceBus.Subscriptions.Messages;
    using NUnit.Framework;
    using Rhino.Mocks;
    using Rhino.Mocks.Interfaces;

    [TestFixture]
    public class How_do_i_watch_the_storage
    {
        MockRepository mocks;
        private ISubscriptionStorage cache;
        private IServiceBus bus;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            cache = mocks.CreateMock<ISubscriptionStorage>();
            bus = mocks.CreateMock<IServiceBus>();
        }

        [TearDown]
        public void TearDown()
        {
            mocks = null;
            cache = null;
            bus = null;
        }

        [Test]
        public void When_first_watching()
        {
            using(mocks.Record())
            {
                bus = mocks.Stub<IServiceBus>();
                cache.SubscriptionChanged += null;
                LastCall.IgnoreArguments();
                Expect.Call(cache.List()).Return(new List<Uri>());
            }
            using(mocks.Playback())
            {
                ClientProxy proxy = new ClientProxy(new MessageQueueEndpoint("msmq://localhost/test"));
                proxy.StartWatching(bus, cache);    
            }
        }

        [Test]
        public void When_responding_to_a_subscription_change()
        {
            IEventRaiser eventRaiser;
            using (mocks.Record())
            {
                bus = mocks.Stub<IServiceBus>();
                cache.SubscriptionChanged += null;
                LastCall.IgnoreArguments();
                eventRaiser = LastCall.GetEventRaiser();

                Expect.Call(cache.List()).Return(new List<Uri>());
                
                
            }
            using (mocks.Playback())
            {
                ClientProxy proxy = new ClientProxy(new MessageQueueEndpoint("msmq://localhost/test"));
                proxy.StartWatching(bus, cache);

                eventRaiser.Raise(cache, new SubscriptionChangedEventArgs(new SubscriptionChange("bob", new Uri("msmq://localhost/bob"), SubscriptionChange.SubscriptionChangeType.Add)));
            }
        }


    }
}
