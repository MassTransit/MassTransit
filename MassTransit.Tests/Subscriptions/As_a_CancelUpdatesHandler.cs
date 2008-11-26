namespace MassTransit.Tests.Subscriptions
{
    using System;
    using MassTransit.Internal;
    using MassTransit.Subscriptions;
    using MassTransit.Subscriptions.Messages;
    using MassTransit.Subscriptions.ServerHandlers;
    using NUnit.Framework;

    [TestFixture]
    public class As_a_CancelUpdatesHandler
        : Specification
    {
        private CancelUpdatesHandler handle;
        private IEndpointResolver _mockResolver;
        private CancelSubscriptionUpdates msgCancel;
        private Uri uri = new Uri("queue:\\bob");

        protected override void Before_each()
        {
            msgCancel = new CancelSubscriptionUpdates(uri);
            _mockResolver = StrictMock<IEndpointResolver>();
            handle = new CancelUpdatesHandler(new FollowerRepository(_mockResolver));
        }


        [Test]
        public void respond_to_update_cancel()
        {
            using (Record())
            {
            }
            using (Playback())
            {
                handle.Consume(msgCancel);
            }
        }
    }
}