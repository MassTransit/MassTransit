namespace MassTransit.ServiceBus.Tests
{
    using System;
    using Messages;
    using NUnit.Framework;
    using Services.Subscriptions.Messages;
    using Services.Subscriptions.Server.Messages;
    using Rhino.Mocks;

    public class ProxyGetsMessages :
        WithStartedProxyService
    {
        Guid _clientId = Guid.NewGuid();
        Uri _controlUri = new Uri("null://local/control");
        Uri _dataUri = new Uri("null://local/data");

        public override void BecauseOf()
        {
            //this would come from the saga?
            var message = new LegacySubscriptionClientAdded {ClientId = _clientId, ControlUri = _controlUri, DataUri = _dataUri};

            Service.Consume(message);
        }

        [Test]
        public void ShouldForwardAndTranslate()
        {
            var message = new AddSubscriptionClient(_clientId, _controlUri, _dataUri); // why isn't this working?
            MockNewSubEndpoint.AssertWasCalled(e=>e.Send(message), c=>c.IgnoreArguments());
        }
    }

    public class Removed :
        WithStartedProxyService
    {
        Guid _clientId = Guid.NewGuid();
        Uri _controlUri = new Uri("null://local/control");
        Uri _dataUri = new Uri("null://local/data");

        public override void BecauseOf()
        {
            //this would come from the saga?
            var message = new LegacySubscriptionClientRemoved() {CorrelationId = _clientId, ControlUri = _controlUri, DataUri = _dataUri };

            Service.Consume(message);
        }

        [Test]
        public void ShouldForwardAndTranslate()
        {
            var message = new RemoveSubscriptionClient(_clientId, _controlUri, _dataUri); // why isn't this working?
            MockNewSubEndpoint.AssertWasCalled(e => e.Send(message), c => c.IgnoreArguments());
        }
    }
}