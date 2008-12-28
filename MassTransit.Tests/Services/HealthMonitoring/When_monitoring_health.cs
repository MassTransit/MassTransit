namespace MassTransit.Tests.Services.HealthMonitoring
{
    using System;
    using Magnum.Common.DateTimeExtensions;
    using MassTransit.Services.HealthMonitoring;
    using MassTransit.Services.HealthMonitoring.Messages;
    using NUnit.Framework;
    using TextFixtures;

    [TestFixture]
    public class When_monitoring_health :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void Starting_should_enable_the_service()
        {
            HealthClient hc = new HealthClient(RemoteBus);
            Assert.IsFalse(hc.Enabled);
            hc.Start();
            Assert.IsTrue(hc.Enabled);
            hc.Stop();
            Assert.IsFalse(hc.Enabled);
            hc.Dispose();
            hc.Dispose();
        }

        [Test]
        public void Calling_dispose_twice_shouldnt_error()
        {
            HealthClient hc = new HealthClient(RemoteBus);
            hc.Start();
            hc.Dispose();
            hc.Dispose();
        }


        [Test]
        public void Should_beat_continously()
        {
            FutureMessage<Heartbeat> fm = new FutureMessage<Heartbeat>();
            int beatCount = 0;
            LocalBus.Subscribe<Heartbeat>(msg =>
                                              {
                                                  beatCount++;
                                                  if (beatCount > 1)
                                                      fm.Set(msg);
                                              });

            HealthClient hc = new HealthClient(RemoteBus, 1);
            hc.Start();

            fm.IsAvailable(3.Seconds())
                .ShouldBeTrue();
        }

        [Test]
        public void Should_respond_to_pings_by_publishing_pongs()
        {
            Guid id = Guid.NewGuid();
            Ping png = new Ping(id);
            FutureMessage<Pong> fm = new FutureMessage<Pong>();

            LocalBus.Subscribe<Pong>(msg =>
                                         {
                                             msg.CorrelationId
                                                 .ShouldEqual(id);
                                             fm.Set(msg);
                                         });


            HealthClient hc = new HealthClient(RemoteBus);
            hc.Consume(png);
            fm.IsAvailable(2.Seconds())
                .ShouldBeTrue();
        }
    }
}