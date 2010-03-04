namespace MassTransit.LegacySupport.Tests.Saga
{
    using Contexts;
    using Messages;
    using NUnit.Framework;
    using ProxyMessages;
    using Rhino.Mocks;
    using TestFramework;

    public class WhenReceivingACancelSubscription :
        GivenAnActiveSaga
    {
        [When]
        public void OldCancelReceived()
        {
            var data = new OldCancelSubscriptionUpdates(CorrelationUri);
            Saga.RaiseEvent(LegacySubscriptionClientSaga.OldCancelSubscriptionUpdates, data);
        }

        [Then]
        public void StateShouldBeCompleted()
        {
            Assert.That(Saga.CurrentState, Is.EqualTo(LegacySubscriptionClientSaga.Completed));
        }

        [Then]
        public void MessageShouldBePublished()
        {
            var msg = new LegacySubscriptionClientAdded
                          {
                              ClientId = CorrelationId,
                              ControlUri = MockBus.Endpoint.Uri,
                              DataUri = CorrelationUri
                          };
            MockBus.AssertWasCalled(bus => bus.Publish(msg));
        }
        
    }
}