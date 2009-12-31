namespace MassTransit.LegacySupport.Tests.Saga
{
    using Contexts;
    using Messages;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ServiceBus.Subscriptions.Messages;

    public class ClientSagaSpecs :
        WithActiveSaga
    {

        public override void BecauseOf()
        {
            var data = new CancelSubscriptionUpdates(CorrelationUri);
            Saga.RaiseEvent(LegacySubscriptionClientSaga.OldCancelSubscriptionUpdates, data);
        }

        [Test]
        public void StateShouldBeActive()
        {
            Assert.That(Saga.CurrentState, Is.EqualTo(LegacySubscriptionClientSaga.Completed));
        }

        [Test]
        public void MessageShouldBePublished()
        {
            var msg = new LegacySubscriptionClientAdded()
                          {
                              ClientId = CorrelationId,
                              ControlUri = MockBus.Endpoint.Uri,
                              DataUri = CorrelationUri
                          };
            MockBus.AssertWasCalled(bus => bus.Publish(msg));
        }
    }
}