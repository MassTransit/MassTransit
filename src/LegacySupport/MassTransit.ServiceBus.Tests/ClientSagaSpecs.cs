namespace MassTransit.ServiceBus.Tests
{
    using System;
    using NUnit.Framework;
    using OldAddSubscription = Subscriptions.Messages.AddSubscription;
    using OldRemoveSubscription = Subscriptions.Messages.RemoveSubscription;
    using OldCacheUpdateRequest = Subscriptions.Messages.CacheUpdateRequest;
    using OldCacheUpdateResponse = Subscriptions.Messages.CacheUpdateResponse;
    using OldCancelSubscriptionUpdates = Subscriptions.Messages.CancelSubscriptionUpdates;

    public class ClientSagaSpecs :
        WithActiveSaga
    {
        Uri _uri = new Uri("http://localhost/bob");

        public override void BecauseOf()
        {
        }

        [Test]
        public void StateShouldBeActive()
        {
            Assert.That(Saga.CurrentState, Is.EqualTo(LegacySubscriptionClientSaga.Active));
        }

        [Test]
        public void NAME()
        {
            Assert.That(Saga.DataUri, Is.EqualTo(_uri));
        }

        //old cache update
        //old cache request
        //old subscription removed

        //old cancel subscription
    }
}