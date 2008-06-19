namespace MassTransit.ServiceBus.Tests.Subscriptions
{
    using System;
    using MassTransit.ServiceBus.Internal;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.ServiceBus.Subscriptions.Messages;
    using MassTransit.ServiceBus.Subscriptions.ServerHandlers;
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