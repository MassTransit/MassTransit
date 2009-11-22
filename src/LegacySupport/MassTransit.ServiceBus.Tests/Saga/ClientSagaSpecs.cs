namespace MassTransit.ServiceBus.Tests.Saga
{
    using Contexts;
    using Messages;
    using NUnit.Framework;
    using OldAddSubscription = Subscriptions.Messages.AddSubscription;
    using OldRemoveSubscription = Subscriptions.Messages.RemoveSubscription;
    using OldCacheUpdateRequest = Subscriptions.Messages.CacheUpdateRequest;
    using OldCacheUpdateResponse = Subscriptions.Messages.CacheUpdateResponse;
    using OldCancelSubscriptionUpdates = Subscriptions.Messages.CancelSubscriptionUpdates;
    using Rhino.Mocks;

    public class ClientSagaSpecs :
        WithActiveSaga
    {

        public override void BecauseOf()
        {
            var data = new OldCancelSubscriptionUpdates(CorrelationUri);
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