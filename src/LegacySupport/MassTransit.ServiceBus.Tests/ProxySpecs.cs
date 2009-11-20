namespace MassTransit.ServiceBus.Tests
{
    using System;
    using NUnit.Framework;

    public class ProxySpecs
    {
        
    }


    public abstract class WithActiveSaga
    {
        public WithActiveSaga()
        {

        }

        [TestFixtureSetUp]
        public void Setup()
        {
            Saga = new LegacySubscriptionClientSaga(CorrelationId);
            Saga.RaiseEvent(LegacySubscriptionClientSaga.OldCacheUpdateRequested);
        }

        public abstract void BecauseOf();
        public Guid CorrelationId { get; private set; }
        public LegacySubscriptionClientSaga Saga { get; private set; }
    }
}