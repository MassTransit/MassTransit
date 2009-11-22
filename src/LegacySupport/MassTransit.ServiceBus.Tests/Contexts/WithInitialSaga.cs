namespace MassTransit.ServiceBus.Tests.Contexts
{
    using System;
    using Internal;
    using NUnit.Framework;
    using Rhino.Mocks;

    public abstract class WithInitialSaga
    {

        [TestFixtureSetUp]
        public void Setup()
        {
            CorrelationUri = new Uri("msmq://bob/fitzgerald");
            CorrelationId = Guid.NewGuid();
            Saga = new LegacySubscriptionClientSaga(CorrelationId);
            MockBus = MockRepository.GenerateStub<IServiceBus>();
            MockBus.Stub(x => x.Endpoint).Return(new NullEndpoint());
            Saga.Bus = MockBus;
            BecauseOf();
        }

        public abstract void BecauseOf();
        public Guid CorrelationId { get; private set; }
        public Uri CorrelationUri { get; private set; }
        public IServiceBus MockBus { get; private set; }
        public LegacySubscriptionClientSaga Saga { get; private set; }
    }
}