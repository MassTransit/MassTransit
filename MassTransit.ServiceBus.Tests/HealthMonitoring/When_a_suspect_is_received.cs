namespace MassTransit.ServiceBus.Tests.HealthMonitoring
{
    using System;
    using System.Threading;
    using MassTransit.ServiceBus.HealthMonitoring;
    using MassTransit.ServiceBus.HealthMonitoring.Messages;
    using MassTransit.ServiceBus.Internal;
    using NUnit.Framework;
    using Rhino.Mocks;
    using Rhino.Mocks.Constraints;

    [TestFixture]
    public class When_a_suspect_is_received :
        Specification
    {
        IServiceBus mockBus;
        IEndpointResolver mockEndpointResolver;
        IEndpoint mockEndpoint;
        IHealthCache mockCache;

        private HealthInformation information;
        private Uri _testUri = new Uri("msmq://localhost/test");

        protected override void Before_each()
        {
            mockBus = DynamicMock<IServiceBus>();
            mockEndpointResolver = DynamicMock<IEndpointResolver>();
            mockEndpoint = DynamicMock<IEndpoint>();
            mockCache = DynamicMock<IHealthCache>();

            information = new HealthInformation(_testUri);
        }
        protected override void After_each()
        {
            mockBus = null;
            mockEndpointResolver = null;
            mockEndpoint = null;
            mockCache = null;
        }

        [Test]
        public void An_Investigation_should_happen()
        {
            Investigator inv = new Investigator(mockBus, mockEndpointResolver, mockCache);

            using (Record())
            {
                Expect.Call(mockEndpointResolver.Resolve(_testUri)).Return(mockEndpoint);
                Expect.Call(delegate
                                {
                                    mockEndpoint.Send(new Ping(inv.CorrelationId), new TimeSpan(0,3,0));
                                }).Constraints(Is.Matching<Ping>(delegate(Ping msg)
                                {
                                    return msg.CorrelationId.Equals(inv.CorrelationId);
                                }), Is.Anything());
            }
            using(Playback())
            {
                inv.Consume(new Suspect(_testUri));
            }
        }

        [Test]
        public void An_investigation_should_happen_and_pong_received()
        {
            Investigator inv = new Investigator(mockBus, mockEndpointResolver, mockCache);

            using (Record())
            {
                Expect.Call(mockEndpointResolver.Resolve(_testUri)).Return(mockEndpoint);
                Expect.Call(delegate
                                {
                                    mockEndpoint.Send(new Ping(inv.CorrelationId), new TimeSpan(0, 3, 0));
                                }).Constraints(Is.Matching<Ping>(delegate(Ping msg)
                                {
                                    return msg.CorrelationId.Equals(inv.CorrelationId);
                                }), Is.Anything());
            }
            using (Playback())
            {
                inv.Consume(new Suspect(_testUri));
                Assert.IsTrue(inv.Enabled);
                inv.Consume(new Pong(inv.CorrelationId, _testUri));
                Assert.IsFalse(inv.Enabled);
            }
        }

        [Test]
        public void When_a_ping_times_out()
        {
            Investigator inv = new Investigator(mockBus, mockEndpointResolver, mockCache, 50);
            ManualResetEvent evt = new ManualResetEvent(false);

            using (Record())
            {
                Expect.Call(mockEndpointResolver.Resolve(_testUri)).Return(mockEndpoint);
                Expect.Call(delegate
                                {
                                    mockEndpoint.Send(new Ping(inv.CorrelationId), new TimeSpan(0, 3, 0));
                                }).Constraints(Is.Matching<Ping>(delegate(Ping msg)
                                {
                                    return msg.CorrelationId.Equals(inv.CorrelationId);
                                }), Is.Anything());
                Expect.Call(delegate { mockBus.Publish(new DownEndpoint(_testUri)); })
                    .Constraints(Is.Matching<DownEndpoint>(delegate(DownEndpoint msg)
                                                               {
                                                                   if (msg.Endpoint != _testUri) return false;
                                                                   evt.Set();
                                                                   return true;
                                                               }));
            }
            using (Playback())
            {
                inv.Consume(new Suspect(_testUri));
                Assert.IsTrue(evt.WaitOne(500, true));
            }
        }
    }
}