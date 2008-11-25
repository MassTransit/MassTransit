namespace MassTransit.Tests.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ServiceBus;
    using ServiceBus.Internal;
    using ServiceBus.Subscriptions;
    using ServiceBus.Subscriptions.Messages;
    using ServiceBus.Subscriptions.ServerHandlers;

    [TestFixture]
    public class As_a_CacheUpdateRequestHandler
        : Specification
    {
        private CacheUpdateRequestHandler handle;
        private ISubscriptionCache _mockCache;
        private FollowerRepository _mockFollower;
        private IEndpointResolver _mockEndpointResolver;

        private CacheUpdateRequest msgUpdate;
        private Uri uri = new Uri("queue:\\bob");


        protected override void Before_each()
        {
            msgUpdate = new CacheUpdateRequest(uri);


            _mockEndpointResolver = StrictMock<IEndpointResolver>();
            _mockCache = StrictMock<ISubscriptionCache>();
            _mockFollower = new FollowerRepository(_mockEndpointResolver);

            handle = new CacheUpdateRequestHandler(_mockFollower, _mockEndpointResolver, _mockCache);
        }


        [Test]
        public void respond_to_cache_updates()
        {
            IEndpoint ep = DynamicMock<IEndpoint>();

            using (Record())
            {
                Expect.Call(_mockCache.List()).Return(new List<Subscription>());

                Expect.Call(_mockEndpointResolver.Resolve(uri)).Return(ep);
                Expect.Call(delegate { ep.Send<CacheUpdateResponse>(null); }).IgnoreArguments();

            }
            using (Playback())
            {
                handle.Consume(msgUpdate);
            }
        }
    }
}