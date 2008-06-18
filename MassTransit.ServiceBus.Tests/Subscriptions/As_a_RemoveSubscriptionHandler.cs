namespace MassTransit.ServiceBus.Tests.Subscriptions
{
    using System;
    using MassTransit.ServiceBus.Internal;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.ServiceBus.Subscriptions.Messages;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class As_a_RemoveSubscriptionHandler
        : Specification
    {
        private RemoveSubscriptionHandler handle;
        private ISubscriptionCache _mockCache;
        private ISubscriptionRepository _mockRepository;
        private FollowerRepository _mockFollower;
        private IEndpointResolver _mockEndpointResolver;


        private RemoveSubscription msgRem;
        private Uri uri = new Uri("queue:\\bob");

        protected override void Before_each()
        {
            _mockEndpointResolver = StrictMock<IEndpointResolver>();
            _mockCache = StrictMock<ISubscriptionCache>();
            _mockRepository = StrictMock<ISubscriptionRepository>();
            _mockFollower = new FollowerRepository(_mockEndpointResolver);
            handle = new RemoveSubscriptionHandler(_mockCache, _mockRepository, _mockFollower);

            msgRem = new RemoveSubscription("bob", uri);
        }

        [Test]
        public void remove_subscriptions_from_messages()
        {
            using (Record())
            {
                Expect.Call(delegate { _mockCache.Remove(null); }).IgnoreArguments();
                Expect.Call(delegate { _mockRepository.Remove(msgRem.Subscription); });
            }
            using (Playback())
            {
                handle.Consume(msgRem);
            }
        }

    }
}