namespace MassTransit.ServiceBus.Tests.HealthMonitoring
{
    using System;
    using System.Threading;
    using Internal;
    using MassTransit.ServiceBus.HealthMonitoring;
    using MassTransit.ServiceBus.HealthMonitoring.Messages;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class When_a_suspect_is_received :
        Specification
    {
        [Test]
        public void An_Investigation_should_happen()
        {
            IServiceBus bus = DynamicMock<IServiceBus>();
            IEndpointResolver er = DynamicMock<IEndpointResolver>();
            IEndpoint ep = DynamicMock<IEndpoint>();
            Investigator inv = new Investigator(bus, er);

            using (Record())
            {
                Expect.Call(er.Resolve(new Uri("msmq://localhost/test"))).Return(ep);
                Expect.Call(delegate
                                {
                                    ep.Send(new Ping(inv.CorrelationId), new TimeSpan(0,3,0));
                                });
            }
            using(Playback())
            {
                inv.Consume(new Suspect(new Uri("msmq://localhost/test")));
            }
        }

        [Test]
        public void When_a_ping_times_out()
        {
            IServiceBus bus = DynamicMock<IServiceBus>();
            IEndpointResolver er = DynamicMock<IEndpointResolver>();
            IEndpoint ep = DynamicMock<IEndpoint>();
            Investigator inv = new Investigator(bus, er);
            Uri u = new Uri("msmq://localhost/test");
                ManualResetEvent evt = new ManualResetEvent(false);

            using (Record())
            {
                Expect.Call(er.Resolve(u)).Return(ep);
                Expect.Call(delegate
                                {
                                    ep.Send(new Ping(inv.CorrelationId), new TimeSpan(0, 3, 0));
                                });
                Expect.Call(delegate { bus.Publish(new DownEndpoint(u)); }); //evt.Set();
            }
            using (Playback())
            {
                inv.Consume(new Suspect(new Uri("msmq://localhost/test")));
                evt.WaitOne(50, true);
            }
        }
    }
}