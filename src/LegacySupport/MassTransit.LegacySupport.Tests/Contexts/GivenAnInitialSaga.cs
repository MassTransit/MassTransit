namespace MassTransit.LegacySupport.Tests.Contexts
{
    using System;
    using Internal;
    using Rhino.Mocks;
    using TestFramework;

    public abstract class GivenAnInitialSaga
    {

        [Given]
        public void Setup()
        {
            CorrelationUri = new Uri("msmq://bob/fitzgerald");
            CorrelationId = Guid.NewGuid();
            Saga = new LegacySubscriptionClientSaga(CorrelationId);
            MockBus = MockRepository.GenerateStub<IServiceBus>();
            MockBus.Stub(x => x.Endpoint).Return(new NullEndpoint());
            Saga.Bus = MockBus;
        }

        public Guid CorrelationId { get; private set; }
        public Uri CorrelationUri { get; private set; }
        public IServiceBus MockBus { get; private set; }
        public LegacySubscriptionClientSaga Saga { get; private set; }
    }
}