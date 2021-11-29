namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [Category("Flaky")]
    [TestFixture]
    public class KillSwitch_Specs :
        ActiveMqTestFixture
    {
        [Test]
        public async Task Should_be_degraded_after_too_many_exceptions()
        {
            Assert.That(await BusControl.WaitForHealthStatus(BusHealthStatus.Healthy, TimeSpan.FromSeconds(10)), Is.EqualTo(BusHealthStatus.Healthy));

            await Task.WhenAll(Enumerable.Range(0, 11).Select(x => Bus.Publish(new BadMessage())));

            Assert.That(await BusControl.WaitForHealthStatus(BusHealthStatus.Degraded, TimeSpan.FromSeconds(15)), Is.EqualTo(BusHealthStatus.Degraded));

            Assert.That(await BusControl.WaitForHealthStatus(BusHealthStatus.Healthy, TimeSpan.FromSeconds(10)), Is.EqualTo(BusHealthStatus.Healthy));

            Assert.That(await ActiveMqTestHarness.Consumed.SelectAsync<BadMessage>().Take(11).Count(), Is.EqualTo(11));

            await Task.WhenAll(Enumerable.Range(0, 20).Select(x => Bus.Publish(new GoodMessage())));

            await Task.Delay(1000);

            Assert.That(await ActiveMqTestHarness.Consumed.SelectAsync<GoodMessage>().Take(20).Count(), Is.EqualTo(20));
        }

        protected override void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
        {
            configurator.UseKillSwitch(options => options
                .SetActivationThreshold(10)
                .SetTripThreshold(0.1)
                .SetRestartTimeout(s: 1));
        }

        protected override void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
            configurator.PrefetchCount = 1;

            configurator.Consumer<BadConsumer>();
        }

        public KillSwitch_Specs()
        {
            TestTimeout = TimeSpan.FromMinutes(1);
            TestInactivityTimeout = TimeSpan.FromSeconds(10);
        }


        class BadConsumer :
            IConsumer<BadMessage>,
            IConsumer<GoodMessage>
        {
            public Task Consume(ConsumeContext<BadMessage> context)
            {
                throw new IntentionalTestException("Trying to trigger the kill switch");
            }

            public Task Consume(ConsumeContext<GoodMessage> context)
            {
                return Task.CompletedTask;
            }
        }


        class GoodMessage
        {
        }


        class BadMessage
        {
        }
    }
}
