namespace MassTransit.Tests.Services.HealthMonitoring
{
    using System;
    using System.Threading;
    using Magnum.Common.DateTimeExtensions;
    using MassTransit.Services.HealthMonitoring.Messages;
    using MassTransit.Services.HealthMonitoring.Server;
    using NUnit.Framework;
    using Rhino.Mocks;
    using TextFixtures;

    [TestFixture]
    public class When_monitoring_heartbeats :
        LoopbackLocalAndRemoteTestFixture
    {
        private HeartbeatMonitor _heartbeatMonitor;
        private Heartbeat message;
        private IHealthCache _theCacheIsntInterestingHere;
        private IHeartbeatTimer _timer;
        private Uri _uri;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            _uri = RemoteBus.Endpoint.Uri;
            _theCacheIsntInterestingHere = MockRepository.GenerateMock<IHealthCache>();
            _timer = MockRepository.GenerateStub<IHeartbeatTimer>();

            _heartbeatMonitor = new HeartbeatMonitor(_theCacheIsntInterestingHere, _timer);
            message = new Heartbeat(1, _uri);
        }

        protected override void TeardownContext()
        {
            _heartbeatMonitor = null;
            _theCacheIsntInterestingHere = null;
            _timer = null;
            base.TeardownContext();
        }

        [Test]
        public void When_a_heartbeat_comes_in_for_the_first_time()
        {
            _heartbeatMonitor = new HeartbeatMonitor(_theCacheIsntInterestingHere, _timer);
            _heartbeatMonitor.Consume(message);

            _timer.AssertWasCalled(x => x.Add(null), o=>o.IgnoreArguments());
        }

        [Test]
        public void When_a_heartbeat_comes_in_again_resets_time()
        {
            _heartbeatMonitor.Consume(message);
            Thread.Sleep(700);
            _heartbeatMonitor.Consume(message);
            Thread.Sleep(700);
        }

        [Test]
        public void When_a_monitor_is_reset_it_can_still_fire()
        {
            FutureMessage<Suspect> fm = new FutureMessage<Suspect>();

            RemoteBus.Subscribe<Suspect>(fm.Set);


            _heartbeatMonitor.Consume(message);
            Thread.Sleep(700);
            _heartbeatMonitor.Consume(message);
            fm.IsAvailable(2.Seconds())
                .ShouldBeTrue();
        }

        [Test]
        public void When_a_heartbeat_is_missed_Suspect_published()
        {
            FutureMessage<Suspect> fm = new FutureMessage<Suspect>();
            RemoteBus.Subscribe<Suspect>(fm.Set);
            
            
                _heartbeatMonitor.Consume(message);
            fm.IsAvailable(2.Seconds())
                .ShouldBeTrue();

            
        }
    }
}