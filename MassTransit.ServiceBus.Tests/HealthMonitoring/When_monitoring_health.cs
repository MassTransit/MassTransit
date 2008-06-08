namespace MassTransit.ServiceBus.Tests.HealthMonitoring
{
    using System;
    using System.Threading;
    using MassTransit.ServiceBus.HealthMonitoring;
    using MassTransit.ServiceBus.HealthMonitoring.Messages;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class When_monitoring_health :
        Specification
    {
        private IServiceBus _mockBus;
        private Uri u = new Uri("msmq://localhost/test");

        protected override void Before_each()
        {
            _mockBus = DynamicMock<IServiceBus>();
        }

        [Test]
        public void Should_beat_continously()
        {
            using(Record())
            {
                Expect.Call(delegate { _mockBus.Publish(new Heartbeat(1, u)); }).Repeat.Twice();
            }
            using(Playback())
            {
                HealthClient hc = new HealthClient(_mockBus, 1);
                hc.Start();
                Thread.Sleep(2050);
            }


        }
    }
}