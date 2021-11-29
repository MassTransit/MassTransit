namespace MassTransit.RabbitMqTransport.Tests
{
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;


    [TestFixture]
    public class Failure_Specs
    {
        [Test]
        [Explicit]
        public async Task Should_properly_fail_on_exclusive_launch()
        {
            var harness1 = new RabbitMqTestHarness();
            harness1.OnConfigureRabbitMqBus += configurator =>
            {
                configurator.OverrideDefaultBusEndpointQueueName("exclusively-yours");
                configurator.Exclusive = true;
            };

            var harness2 = new RabbitMqTestHarness();
            harness2.OnConfigureRabbitMqBus += configurator =>
            {
                configurator.OverrideDefaultBusEndpointQueueName("exclusively-yours");
                configurator.Exclusive = true;
            };

            await harness1.Start();
            try
            {
                Assert.That(async () => await harness2.Start(), Throws.TypeOf<RabbitMqConnectionException>());

                await harness2.Stop();
            }
            finally
            {
                await Task.Delay(1000);

                await harness1.Stop();
            }
        }
    }
}
