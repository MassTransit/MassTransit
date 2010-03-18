namespace MassTransit.LegacySupport.Tests.Saga
{
    using System;
    using Contexts;
    using Messages;
    using NUnit.Framework;
    using ProxyMessages;
    using Rhino.Mocks;
    using TestFramework;

    public class WhenReceivingASubscriptionAdded :
        GivenAnInitialSaga
    {
        [When]
        public void MessageReceived()
        {
            var data = new OldAddSubscription(new Subscription("msg",Guid.NewGuid().ToString(),new Uri("msmq://localhost/bob")));

            Saga.RaiseEvent(LegacySubscriptionClientSaga.OldSubscriptionAdded, data);
        }

        [Then]
        public void StateShouldBeActive()
        {
            Assert.That(Saga.CurrentState, Is.EqualTo(LegacySubscriptionClientSaga.Active));
        }

        [Then, Ignore("What am I doing wrong?")]
        public void Then()
        {
            var message = new LegacySubscriptionClientAdded
                              {
                                  ClientId = CorrelationId,
                                  ControlUri = MockBus.Endpoint.Uri,
                                  DataUri = CorrelationUri,
                              };

            MockBus.AssertWasCalled(bus => bus.Publish(message));

        }

    }
}