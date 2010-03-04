namespace MassTransit.LegacySupport.Tests.Proxy
{
    using Contexts;
    using Rhino.Mocks;
    using TestFramework;

    public class WhenProxyStarts :
        GivenAProxyService
    {
        [When]
        public void BecauseOf()
        {
            Service.Start();
        }

        [Then]
        public void ShouldSubscribeItself()
        {
            MockBus.AssertWasCalled(b=>b.Subscribe(Service));
        }

        [Then]
        public void ShouldSubscribeToSaga()
        {
            MockBus.AssertWasCalled(b=>b.Subscribe<LegacySubscriptionClientSaga>());
        }

        [Then]
        public void ShouldCaptureTheNewEndpoint()
        {
            MockEndpointFactory.AssertWasCalled(e=>e.GetEndpoint("msmq://localhost/new_subservice"));
        }
    }
}