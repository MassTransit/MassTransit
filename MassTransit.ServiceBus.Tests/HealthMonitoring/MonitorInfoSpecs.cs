namespace MassTransit.ServiceBus.Tests.HealthMonitoring
{
    using System;
    using System.Threading;
    using MassTransit.ServiceBus.HealthMonitoring;
    using MassTransit.ServiceBus.HealthMonitoring.Messages;
    using MassTransit.ServiceBus.Internal;
    using MassTransit.ServiceBus.Subscriptions;
    using NUnit.Framework;
    using Transports;

	public class MonitorInfoSpecs :
        Specification
    {
        [Test]
        public void MonitorInfoTimer()
        {
        	ManualResetEvent expired = new ManualResetEvent(false);

            Uri u = new Uri("msmq://localhost/ddd");

			MonitorInfo m = new MonitorInfo(u, 1, delegate { expired.Set(); });

        	Assert.IsTrue(expired.WaitOne(TimeSpan.FromSeconds(2), true));
        }

        [Test]
        [Ignore("Needs new castle object builder")]
        public void bob()
        {
						EndpointResolver.AddTransport(typeof(LoopbackEndpoint));

        	EndpointResolver _endpointResolver = new EndpointResolver();
        	IEndpoint _mockServiceBusEndPoint = _endpointResolver.Resolve(new Uri("loopback://localhost/test"));

			ServiceBus bus = new ServiceBus(_mockServiceBusEndPoint, DynamicMock<IObjectBuilder>());
            bus.AddComponent<HeartbeatMonitor>();
            bus.Dispatch(new Heartbeat(3, new Uri("msmq://localhost/ddd")));
        }
    }
}