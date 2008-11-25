namespace MassTransit.Tests.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ServiceBus;
    using ServiceBus.Internal;
    using ServiceBus.Subscriptions;
    using ServiceBus.Subscriptions.ServerHandlers;

    [TestFixture]
    public class As_a_SubscriptionService :
        Specification
    {
        private IServiceBus mockBus;
        private ISubscriptionCache mockCache;
        private ISubscriptionRepository mockRepository;
        private SubscriptionService srv;
        private IEndpointResolver mockEndpointResolver;

		

        private readonly Uri uri = new Uri("queue:\\bob");

        protected override void Before_each()
        {
            mockBus = StrictMock<IServiceBus>();
            mockRepository = StrictMock<ISubscriptionRepository>();
            mockEndpointResolver = StrictMock<IEndpointResolver>();
            mockCache = DynamicMock<ISubscriptionCache>();

            IEndpoint mockEndpoint = DynamicMock<IEndpoint>();
            SetupResult.For(mockBus.Endpoint).Return(mockEndpoint);
            SetupResult.For(mockEndpoint.Uri).Return(new Uri("queue://bus"));

            srv = new SubscriptionService(mockBus, mockCache, mockRepository);
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

            using (Record())
            {
                Expect.Call(mockRepository.List()).Return(enumer);
                Expect.Call(delegate { mockBus.Subscribe<AddSubscriptionHandler>(); });
                Expect.Call(delegate { mockBus.Subscribe<RemoveSubscriptionHandler>(); });
                Expect.Call(delegate { mockBus.Subscribe<CacheUpdateRequestHandler>(); });
                Expect.Call(delegate { mockBus.Subscribe<CancelUpdatesHandler>(); });
            }
            using (Playback())
            {
                srv.Start();
            }
        }




        [Test]
        public void be_startable_with_stored_subscriptions()
        {
            IList<Subscription> enumer = new List<Subscription>();
            enumer.Add(new Subscription("bob", uri));

            using (Record())
            {
                Expect.Call(mockRepository.List()).Return(enumer);
                Expect.Call(delegate { mockCache.Add(null); }).IgnoreArguments();

                Expect.Call(delegate { mockBus.Subscribe<AddSubscriptionHandler>(); });
                Expect.Call(delegate { mockBus.Subscribe<RemoveSubscriptionHandler>(); });
                Expect.Call(delegate { mockBus.Subscribe<CacheUpdateRequestHandler>(); });
                Expect.Call(delegate { mockBus.Subscribe<CancelUpdatesHandler>(); });
            }
            using (Playback())
            {
                srv.Start();
            }
        }

        [Test]
        public void be_stopable()
        {
            using (Record())
            {
                Expect.Call(delegate { mockBus.Unsubscribe<AddSubscriptionHandler>(); });
                Expect.Call(delegate { mockBus.Unsubscribe<RemoveSubscriptionHandler>(); });
                Expect.Call(delegate { mockBus.Unsubscribe<CacheUpdateRequestHandler>(); });
                Expect.Call(delegate { mockBus.Unsubscribe<CancelUpdatesHandler>(); });
            }
            using (Playback())
            {
                srv.Stop();
            }
        }


 




        [Test]
        public void Calling_dispose_twice_should_be_safe()
        {
            using (Record())
            {
                this.mockBus.Dispose();
                this.mockRepository.Dispose();
                this.mockBus.Dispose();
                this.mockRepository.Dispose();

            }
            using (Playback())
            {
                srv.Dispose();
                srv.Dispose();
            }
        }
    }
}