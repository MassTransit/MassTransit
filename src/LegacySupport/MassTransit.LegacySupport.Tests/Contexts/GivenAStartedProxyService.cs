namespace MassTransit.LegacySupport.Tests.Contexts
{
    using Internal;
    using MassTransit.Saga;
    using Rhino.Mocks;
    using TestFramework;

    public abstract class GivenAStartedProxyService
    {
        public LegacySubscriptionProxyService Service { get; private set; }
        public IServiceBus MockBus { get; private set; }
        public IEndpointFactory MockEndpointFactory { get; private set; }
        public ISagaRepository<LegacySubscriptionClientSaga> MockRepo { get; private set; }
        public IEndpoint MockNewSubEndpoint { get; private set; }

        [Given]
        public void EstablishContext()
        {
            MockBus = MockRepository.GenerateStub<IServiceBus>();
            MockBus.Stub(b => b.Endpoint).Return(new NullEndpoint());
            MockEndpointFactory = MockRepository.GenerateStub<IEndpointFactory>();
            MockRepo = MockRepository.GenerateStub<ISagaRepository<LegacySubscriptionClientSaga>>();
            MockNewSubEndpoint = MockRepository.GenerateStub<IEndpoint>();

            MockEndpointFactory.Stub(e => e.GetEndpoint("msmq://localhost/new_subservice")).Return(MockNewSubEndpoint);

            Service = new LegacySubscriptionProxyService(MockRepo, MockEndpointFactory, MockBus);
            Service.Start();
        }
    }
}