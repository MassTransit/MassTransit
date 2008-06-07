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
            bool _x = false;
            Uri u = new Uri("msmq://localhost/ddd");
            MonitorInfo m = new MonitorInfo(u, 1, delegate { _x = true; });

            Thread.Sleep(1100);

            Assert.IsTrue(_x);
        }
    }
}