namespace MassTransit.ServiceBus.Tests.HealthMonitoring
{
    using System;
    using System.Threading;
    using NUnit.Framework;
    using Rhino.Mocks;
    using Rhino.Mocks.Constraints;
    using Services.HealthMonitoring;
    using Services.HealthMonitoring.Messages;

    [TestFixture]
    public class When_monitoring_heartbeats :
        Specification
    {
        private HeartbeatMonitor hm;
        private IServiceBus _bus;
        private Heartbeat message;
        private Uri u;
        private IHealthCache _theCacheIsntInterestingHere;
        private IHeartbeatTimer _timer;

        protected override void Before_each()
        {
            u = new Uri("msmq://localhost/test");
            _bus = StrictMock<IServiceBus>();
            _theCacheIsntInterestingHere = DynamicMock<IHealthCache>();
            _timer = DynamicMock<IHeartbeatTimer>();
            hm = new HeartbeatMonitor(_theCacheIsntInterestingHere, _timer);
            message = new Heartbeat(1, u);
        }

        protected override void After_each()
        {
            _bus = null;
            hm = null;
            _theCacheIsntInterestingHere = null;
            _timer = null;
        }

        [Test]
        public void When_a_heartbeat_comes_in_for_the_first_time()
        {
            //Assert.IsFalse(hm.AmIWatchingYou(u));
            var timer = MockRepository.GenerateStub<IHeartbeatTimer>();
            hm = new HeartbeatMonitor(_theCacheIsntInterestingHere, timer);
            hm.Consume(message);

            timer.AssertWasCalled(x => x.Add(null), o=>o.IgnoreArguments());
            //Assert.IsTrue(hm.AmIWatchingYou(u));
        }

        [Test]
        public void When_a_heartbeat_comes_in_again_resets_time()
        {
            hm.Consume(message);
            Thread.Sleep(700);
            hm.Consume(message);
            Thread.Sleep(700);
        }

        [Test]
        public void When_a_monitor_is_reset_it_can_still_fire()
        {
            ManualResetEvent evt = new ManualResetEvent(false);
            using (Record())
            {
                Expect.Call(delegate { _bus.Publish(new Suspect(u)); })
                    .Constraints(Is.Matching<Suspect>(delegate(Suspect msg)
                                                          {
                                                              if (msg.EndpointUri != u)
                                                                  return false;

                                                              evt.Set();

                                                              return true;
                                                          }));
            }
            using (Playback())
            {
                hm.Consume(message);
                Thread.Sleep(700);
                hm.Consume(message);
                Assert.IsTrue(evt.WaitOne(1050, true));
            }
        }

        [Test]
        public void When_a_heartbeat_is_missed_Suspect_published()
        {
            ManualResetEvent evt = new ManualResetEvent(false);
            using(Record())
            {
                Expect.Call(delegate { _bus.Publish(new Suspect(u)); })
                    .Constraints(Is.Matching<Suspect>(delegate(Suspect msg)
                                                          {
                                                              if (msg.EndpointUri != u)
                                                                  return false;

                                                              evt.Set();

                                                              return true;
                                                          }));
            }
            using(Playback())
            {
                hm.Consume(message);
                Assert.IsTrue(evt.WaitOne(1050, true));
            }
        }
    }
}