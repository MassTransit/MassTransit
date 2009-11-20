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

            var data = new OldCancelSubscriptionUpdates(_uri);
            Saga.RaiseEvent(LegacySubscriptionClientSaga.OldCancelSubscriptionUpdates,data);
        }

        [Test]
        public void NAME()
        {
            Assert.That(Saga.CorrelationId, Is.EqualTo(CorrelationId));
        }

        //old cache update
        //old cache request
        //old subscription removed

        //old cancel subscription
    }
}