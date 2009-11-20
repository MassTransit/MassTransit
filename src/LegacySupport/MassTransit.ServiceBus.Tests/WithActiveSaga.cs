namespace MassTransit.ServiceBus.Tests
{
    using System;
    using Internal;
    using NUnit.Framework;
    using OldAddSubscription = Subscriptions.Messages.AddSubscription;
    using OldRemoveSubscription = Subscriptions.Messages.RemoveSubscription;
    using OldCacheUpdateRequest = Subscriptions.Messages.CacheUpdateRequest;
    using OldCacheUpdateResponse = Subscriptions.Messages.CacheUpdateResponse;
    using OldCancelSubscriptionUpdates = Subscriptions.Messages.CancelSubscriptionUpdates;

    public abstract class WithActiveSaga
    {

        [TestFixtureSetUp]
        public void Setup()
        {
            CorrelationUri = new Uri("msmq://bob/fitzgerald");
            Saga = new LegacySubscriptionClientSaga(CorrelationId);
            Saga.Bus = new NullServiceBus();
            var data = new OldCacheUpdateRequest(CorrelationUri);
            Saga.RaiseEvent(LegacySubscriptionClientSaga.OldCacheUpdateRequested, data);
            BecauseOf();
        }

        public abstract void BecauseOf();
        public Guid CorrelationId { get; private set; }
        public Uri CorrelationUri { get; private set; }
        public LegacySubscriptionClientSaga Saga { get; private set; }
    }
}