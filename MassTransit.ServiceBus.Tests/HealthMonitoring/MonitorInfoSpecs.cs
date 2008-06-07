namespace MassTransit.ServiceBus.Tests.HealthMonitoring
{
    using System;
    using System.Threading;
    using MassTransit.ServiceBus.HealthMonitoring;
    using NUnit.Framework;

    public class MonitorInfoSpecs :
        Specification
    {
        [Test]
        public void NAME()
        {
        	ManualResetEvent expired = new ManualResetEvent(false);

            Uri u = new Uri("msmq://localhost/ddd");

			MonitorInfo m = new MonitorInfo(u, 1, delegate { expired.Set(); });

        	Assert.IsTrue(expired.WaitOne(TimeSpan.FromSeconds(2), true));
        }
    }
}