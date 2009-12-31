namespace MassTransit.LegacySupport.Tests.Contexts
{
    using MassTransit.Saga;
    using NUnit.Framework;
    using Rhino.Mocks;

    public abstract class WithProxyService
    {
        public LegacySubscriptionProxyService Service { get; private set; }
        public IServiceBus MockBus { get; private set; }
        public IEndpointFactory MockEndpointFactory { get; private set; }
        public ISagaRepository<LegacySubscriptionClientSaga> MockRepo { get; private set; }

        [TestFixtureSetUp]
        public void EstablishContext()
        {
            MockBus = MockRepository.GenerateStub<IServiceBus>();
            MockEndpointFactory = MockRepository.GenerateStub<IEndpointFactory>();
            MockRepo = MockRepository.GenerateStub<ISagaRepository<LegacySubscriptionClientSaga>>();

            Service = new LegacySubscriptionProxyService(MockRepo, MockEndpointFactory, MockBus);
            BecauseOf();
        }

        public abstract void BecauseOf();
    }
}