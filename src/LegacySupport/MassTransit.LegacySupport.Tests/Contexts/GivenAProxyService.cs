namespace MassTransit.LegacySupport.Tests.Contexts
{
    using MassTransit.Saga;
    using Rhino.Mocks;
    using TestFramework;

    public abstract class GivenAProxyService
    {
        public LegacySubscriptionProxyService Service { get; private set; }
        public IServiceBus MockBus { get; private set; }
        public IEndpointFactory MockEndpointFactory { get; private set; }
        public ISagaRepository<LegacySubscriptionClientSaga> MockRepo { get; private set; }

        [Given]
        public void EstablishContext()
        {
            MockBus = MockRepository.GenerateStub<IServiceBus>();
            MockEndpointFactory = MockRepository.GenerateStub<IEndpointFactory>();
            MockRepo = MockRepository.GenerateStub<ISagaRepository<LegacySubscriptionClientSaga>>();

            Service = new LegacySubscriptionProxyService(MockRepo, MockEndpointFactory, MockBus);
        }
    }
}