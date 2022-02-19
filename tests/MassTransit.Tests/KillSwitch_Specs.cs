namespace MassTransit.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    [Category("Flaky")]
    public class KillSwitch_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_degraded_after_too_many_exceptions()
        {
            Assert.That(await BusControl.WaitForHealthStatus(BusHealthStatus.Healthy, TimeSpan.FromSeconds(10)), Is.EqualTo(BusHealthStatus.Healthy));

            await Task.WhenAll(Enumerable.Range(0, 20).Select(x => Bus.Publish(new BadMessage())));

            Assert.That(await BusControl.WaitForHealthStatus(BusHealthStatus.Degraded, TimeSpan.FromSeconds(15)), Is.EqualTo(BusHealthStatus.Degraded));

            Assert.That(await BusControl.WaitForHealthStatus(BusHealthStatus.Healthy, TimeSpan.FromSeconds(10)), Is.EqualTo(BusHealthStatus.Healthy));

            await Task.WhenAll(Enumerable.Range(0, 20).Select(x => Bus.Publish(new GoodMessage())));

            Assert.That(await InMemoryTestHarness.Consumed.SelectAsync<BadMessage>().Take(20).Count(), Is.EqualTo(20));

            Assert.That(await InMemoryTestHarness.Consumed.SelectAsync<GoodMessage>().Take(20).Count(), Is.EqualTo(20));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseKillSwitch(options => options
                .SetActivationThreshold(10)
                .SetTripThreshold(10)
                .SetRestartTimeout(s: 1));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConcurrentMessageLimit = 20;
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
