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
        LocalAndRemoteTestContext
    {
        IHealthCache _mockCache;
        private IEndpoint _mockEndpoint;
        private IEndpointResolver _mockResolver;
            Investigator _investigator;

        private HealthInformation information;

        protected override void Before_each()
        {
            _mockCache = Mock<IHealthCache>();
            _mockEndpoint = StrictMock<IEndpoint>();
            _mockResolver = StrictMock<IEndpointResolver>();
            SetupResult.For(_mockResolver.Resolve(null)).Return(_mockEndpoint).IgnoreArguments();

        	information = new HealthInformation(RemoteBus.Endpoint.Uri, 3);
            _investigator = new Investigator(LocalBus, _mockResolver, _mockCache);
        }
        protected override void After_each()
        {
            _mockCache = null;
            _mockEndpoint = null;
            _mockResolver = null;
            _investigator = null;
        }

        [Test]
        public void An_Investigation_should_happen()
        {

            using (Record)
            {
                Expect.Call(delegate
                                {
                                    _mockEndpoint.Send(new Ping(_investigator.CorrelationId), new TimeSpan(0, 3, 0));
                                }).Constraints(Is.Matching<Ping>(delegate(Ping msg)
                                {
                                    return msg.CorrelationId.Equals(_investigator.CorrelationId);
                                }), Is.Anything());
            }
            using(Playback)
            {
                _investigator.Consume(new Suspect(RemoteBus.Endpoint.Uri));
            }
        }

        [Test]
        public void An_investigation_should_happen_and_pong_received()
        {

            using (Record)
            {
                Expect.Call(delegate
                                {
                                    _mockEndpoint.Send(new Ping(_investigator.CorrelationId), new TimeSpan(0, 3, 0));
                                }).Constraints(Is.Matching<Ping>(delegate(Ping msg)
                                {
                                    return msg.CorrelationId.Equals(_investigator.CorrelationId);
                                }), Is.Anything());
            }
            using (Playback)
            {
                _investigator.Consume(new Suspect(RemoteBus.Endpoint.Uri));
                Assert.IsTrue(_investigator.Enabled);
                _investigator.Consume(new Pong(_investigator.CorrelationId, RemoteBus.Endpoint.Uri));
                Assert.IsFalse(_investigator.Enabled);
            }
        }

        [Test]
        public void When_a_ping_times_out()
        {
            ManualResetEvent evt = new ManualResetEvent(false);
            IServiceBus mockBus = StrictMock<IServiceBus>();
            _investigator = new Investigator(mockBus, _mockResolver, _mockCache, 20);

            using (Record)
            {
                Expect.Call(delegate
                                {
                                    _mockEndpoint.Send(new Ping(_investigator.CorrelationId), new TimeSpan(0, 3, 0));
                                }).Constraints(Is.Matching(delegate(Ping msg)
                                                               {
                                                                   return
                                                                       msg.CorrelationId.Equals(
                                                                           _investigator.CorrelationId);
                                                               }), Is.Anything());
                Expect.Call(delegate { mockBus.Publish(new DownEndpoint(RemoteBus.Endpoint.Uri)); })
                    .Constraints(Is.Matching(delegate(DownEndpoint msg)
                                                 {
                                                     if (msg.Endpoint != RemoteBus.Endpoint.Uri) return false;
                                                     evt.Set();
                                                     return true;
                                                 }));
            }
            using (Playback)
            {
                _investigator.Consume(new Suspect(RemoteBus.Endpoint.Uri));
                Assert.IsTrue(evt.WaitOne(2000, true),"event did not complete correctly");
            }
        }
    }
}