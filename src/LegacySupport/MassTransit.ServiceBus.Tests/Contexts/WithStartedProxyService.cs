namespace MassTransit.ServiceBus.Tests
{
    using NUnit.Framework;
    using Rhino.Mocks;
    using Saga;

    public abstract class WithStartedProxyService
    {
        public LegacySubscriptionProxyService Service { get; private set; }
        public IServiceBus MockBus { get; private set; }
        public IEndpointFactory MockEndpointFactory { get; private set; }
        public ISagaRepository<LegacySubscriptionClientSaga> MockRepo { get; private set; }
        public IEndpoint MockNewSubEndpoint { get; private set; }

        [TestFixtureSetUp]
        public void EstablishContext()
        {
            MockBus = MockRepository.GenerateStub<IServiceBus>();
            MockEndpointFactory = MockRepository.GenerateStub<IEndpointFactory>();
            MockRepo = MockRepository.GenerateStub<ISagaRepository<LegacySubscriptionClientSaga>>();
            MockNewSubEndpoint = MockRepository.GenerateStub<IEndpoint>();

            MockEndpointFactory.Stub(e => e.GetEndpoint("msmq://localhost/new_subservice")).Return(MockNewSubEndpoint);

            Service = new LegacySubscriptionProxyService(MockRepo, MockEndpointFactory, MockBus);
            Service.Start();
            BecauseOf();
        }

        public abstract void BecauseOf();
    }
}