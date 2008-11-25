namespace MassTransit.Tests.Subscriptions
{
    using System;
    using MassTransit.Internal;
    using MassTransit.Subscriptions;
    using MassTransit.Subscriptions.Messages;
    using MassTransit.Subscriptions.ServerHandlers;
    using NUnit.Framework;
    using Rhino.Mocks;
    

    [TestFixture]
    public class As_an_AddSubscriptionHandler
        : Specification
    {
        private AddSubscriptionHandler handle;
        private ISubscriptionCache _mockCache;
        private ISubscriptionRepository _mockRepository;
        private FollowerRepository _mockFollower;
        private IEndpointResolver _mockEndpointResolver;


        private AddSubscription msgAdd;
        private Uri uri = new Uri("queue:\\bob");

        protected override void Before_each()
        {
            _mockEndpointResolver = StrictMock<IEndpointResolver>();
            _mockCache = StrictMock<ISubscriptionCache>();
            _mockRepository = StrictMock<ISubscriptionRepository>();
            _mockFollower = new FollowerRepository(_mockEndpointResolver);
            handle = new AddSubscriptionHandler(_mockCache, _mockRepository, _mockFollower);

            msgAdd = new AddSubscription("bob", uri);
        }

        [Test]
        public void add_subscriptions_from_messages()
        {
            using (Record())
            {
                Expect.Call(delegate { _mockCache.Add(null); }).IgnoreArguments();
                Expect.Call(delegate { _mockRepository.Save(msgAdd.Subscription); });
            }
            using (Playback())
            {
                handle.Consume(msgAdd);
            }
        }

    }
}