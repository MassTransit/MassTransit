namespace MassTransit.ServiceBus.Tests.Proxy
{
    using System;
    using Contexts;
    using Messages;
    using NUnit.Framework;
    using Rhino.Mocks;
    using Services.Subscriptions.Messages;

    public class WhenProxyGetsLosesLegacyClient :
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