namespace MassTransit.ServiceBus.Tests.HealthMonitoring
{
    using System;
    using MassTransit.ServiceBus.HealthMonitoring;
    using MassTransit.ServiceBus.HealthMonitoring.Messages;
    using MassTransit.ServiceBus.Subscriptions;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class When_an_endpoint_goes_down :
        Specification
    {
        [Test]
        public void An_Investigation_should_happen()
        {
            IEndpoint ep = StrictMock<IEndpoint>();
            ISubscriptionCache sc = new LocalSubscriptionCache();
            IObjectBuilder ob = StrictMock<IObjectBuilder>();

            ServiceBus bus = new ServiceBus(ep, sc, ob);
            HealthService hs = new HealthService(bus);
            hs.Start();

            using(Record())
            {
                Expect.Call(ep.Receive(new TimeSpan(0,0,10), delegate { return true; }))
                    .Return(new Suspect(new Uri("msmq://localhost/test")))
                    .IgnoreArguments();
            }
            using(Playback())
            {
                bus.Dispatch(new Suspect(new Uri("msmq://localhost/test")));
            }
            
            

        }
    }
}