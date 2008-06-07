namespace MassTransit.ServiceBus.Tests.HealthMonitoring
{
    using System;
    using MassTransit.ServiceBus.HealthMonitoring;
    using MassTransit.ServiceBus.HealthMonitoring.Messages;
    using MassTransit.ServiceBus.Subscriptions;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class When_an_endpoint_goes_down :
        Specification
    {
        [Test]
        public void An_Investigation_should_happen()
        {
            IEndpoint ep = StaticMock<IEndpoint>();
            ISubscriptionCache sc = DynamicMock<ISubscriptionCache>();
            IObjectBuilder ob = StaticMock<IObjectBuilder>();

            ServiceBus bus = new ServiceBus(ep, sc, ob);
            HealthService hs = new HealthService(bus);

            bus.Dispatch(new Suspect(new Uri("msmq://localhost/test")));
            

        }
    }
}