namespace MassTransit.Tests.HealthMonitoring
{
    using System;
    using System.Threading;
    using MassTransit.Internal;
    using MassTransit.Services.HealthMonitoring;
    using MassTransit.Services.HealthMonitoring.Messages;
    using NUnit.Framework;
    using Rhino.Mocks;
    using Rhino.Mocks.Constraints;
    using TextFixtures;

    [TestFixture]
    public class When_a_suspect_is_received :
        LoopbackLocalAndRemoteTestFixture
    {
        IHealthCache _mockCache;
        private IEndpoint _mockEndpoint;
        private IEndpointFactory _mockResolver;
        Investigator _investigator;

        private HealthInformation information;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            _mockCache = MockRepository.GenerateMock<IHealthCache>();
            _mockEndpoint = MockRepository.GenerateMock<IEndpoint>();
            _mockResolver = MockRepository.GenerateMock<IEndpointFactory>();
            _mockResolver.Stub(x => x.GetEndpoint((Uri)null)).Return(_mockEndpoint).IgnoreArguments();

            information = new HealthInformation(RemoteBus.Endpoint.Uri, 3);
            _investigator = new Investigator(LocalBus, _mockResolver, _mockCache);
        }

        protected override void TeardownContext()
        {
            base.TeardownContext();

            _mockCache = null;
            _mockEndpoint = null;
            _mockResolver = null;
            _investigator = null;
        }

        [Test]
        public void An_Investigation_should_happen()
        {
            _mockEndpoint.Expect(x => x.Send(new Ping(_investigator.CorrelationId), new TimeSpan(0, 3, 0)))
                .Constraints(Is.Matching<Ping>(msg =>
                                               msg.CorrelationId.Equals(_investigator.CorrelationId)), Is.Anything());

            _investigator.Consume(new Suspect(RemoteBus.Endpoint.Uri));

            _mockEndpoint.VerifyAllExpectations();
        }

        [Test]
        public void An_investigation_should_happen_and_pong_received()
        {

            _mockEndpoint.Expect(x=>x.Send(new Ping(_investigator.CorrelationId), new TimeSpan(0, 3, 0)))
                .Constraints(Is.Matching<Ping>(msg => 
                    msg.CorrelationId.Equals(_investigator.CorrelationId)), Is.Anything());

            _investigator.Consume(new Suspect(RemoteBus.Endpoint.Uri));
            _investigator.Enabled
                .ShouldBeTrue();
            
            _investigator.Consume(new Pong(_investigator.CorrelationId, RemoteBus.Endpoint.Uri));
            _investigator.Enabled
                .ShouldBeFalse();
            

            _mockEndpoint.VerifyAllExpectations();
        }

        [Test]
        public void When_a_ping_times_out()
        {
            ManualResetEvent evt = new ManualResetEvent(false);
            IServiceBus mockBus = MockRepository.GenerateMock<IServiceBus>();
            _investigator = new Investigator(mockBus, _mockResolver, _mockCache, 20);

            _mockEndpoint.Expect(x=>x.Send(new Ping(_investigator.CorrelationId), new TimeSpan(0, 3, 0)))
                .Constraints(Is.Matching((Ping msg) => msg.CorrelationId.Equals(
                                                           _investigator.CorrelationId)), Is.Anything());


            mockBus.Expect(x => x.Publish(new DownEndpoint(RemoteBus.Endpoint.Uri)))
                    .Constraints(Is.Matching(delegate(DownEndpoint msg)
                                                 {
                                                     if (msg.Endpoint != RemoteBus.Endpoint.Uri) return false;
                                                     evt.Set();
                                                     return true;
                                                 }));

                _investigator.Consume(new Suspect(RemoteBus.Endpoint.Uri));
            
                Assert.IsTrue(evt.WaitOne(2000, true),"event did not complete correctly");
            
            _mockEndpoint.VerifyAllExpectations();
            mockBus.VerifyAllExpectations();
            
        }
    }
}