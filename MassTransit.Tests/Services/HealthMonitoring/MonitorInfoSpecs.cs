namespace MassTransit.Tests.Services.HealthMonitoring
{
    using System;
    using System.Threading;
    using MassTransit.Services.HealthMonitoring;
    using MassTransit.Services.HealthMonitoring.Messages;
    using NUnit.Framework;
    using TextFixtures;

    public class MonitorInfoSpecs :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void MonitorInfoTimer()
        {
            ManualResetEvent expired = new ManualResetEvent(false);

            Uri u = new Uri("loopback://localhost/ddd");

            MonitorInfo m = new MonitorInfo(u, 1, (info)=> expired.Set() );

            Assert.IsTrue(expired.WaitOne(TimeSpan.FromSeconds(2), true));
        }

        [Test]
        public void bob()
        {
            LocalBus.Subscribe<HeartbeatMonitor>();
            RemoteBus.Publish(new Heartbeat(3, new Uri("loopback://localhost/ddd")));
        }
    }
}