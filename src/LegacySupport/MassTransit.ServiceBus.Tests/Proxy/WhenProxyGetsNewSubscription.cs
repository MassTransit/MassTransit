namespace MassTransit.LegacySupport.Tests.Proxy
{
    using System;
    using System.Collections.Generic;
    using Contexts;
    using Internal;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ServiceBus.Subscriptions.Messages;
    using Services.Subscriptions.Messages;
    using Services.Subscriptions.Server.Messages;

    public class WhenProxyGetsNewSubscription :
        WithStartedProxyService
    {
        Guid _clientId = Guid.NewGuid();
        Uri _controlUri = new Uri("null://local/control");
        Uri _dataUri = new Uri("null://local/data");

        public override void BecauseOf()
        {
            var info = new SubscriptionInformation(_clientId, 0, "", "", _dataUri);
            var message = new SubscriptionAdded() { Subscription = info };

            var saga = new LegacySubscriptionClientSaga(_clientId);
            saga.Bus = new NullServiceBus();

            var data = new CacheUpdateRequest(_dataUri);

            saga.RaiseEvent(LegacySubscriptionClientSaga.OldCacheUpdateRequested, data);
            IEnumerable<LegacySubscriptionClientSaga> sagas = new List<LegacySubscriptionClientSaga>()
                                                                  {
                                                                      saga
                                                                  };
            
            MockRepo.Stub(r => r.Where(s => s.CurrentState == LegacySubscriptionClientSaga.Active)).IgnoreArguments().Return(sagas);
            MockEndpointFactory.Expect(e => e.GetEndpoint(saga.Bus.Endpoint.Uri)).Return(saga.Bus.Endpoint);
           

            Service.Consume(message);
        }

        [Test]
        public void ShouldQueryRepo()
        {
            MockRepo.AssertWasCalled(r => r.Where(x => true), c=>c.IgnoreArguments());
        }

        [Test]
        public void ShouldForwardMessage()
        {
            MockEndpointFactory.VerifyAllExpectations();
        }
    }
}