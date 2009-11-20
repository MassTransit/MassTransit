namespace MassTransit.ServiceBus.Tests
{
    using System;
    using NUnit.Framework;

    public abstract class WithActiveSaga
    {

        [TestFixtureSetUp]
        public void Setup()
        {
            CorrelationUri = new Uri("msmq://bob/fitzgerald");
            Saga = new LegacySubscriptionClientSaga(CorrelationId);
            
            Saga.RaiseEvent(LegacySubscriptionClientSaga.OldCacheUpdateRequested, CorrelationUri);
            BecauseOf();
        }

        public abstract void BecauseOf();
        public Guid CorrelationId { get; private set; }
        public Uri CorrelationUri { get; private set; }
        public LegacySubscriptionClientSaga Saga { get; private set; }
    }
}