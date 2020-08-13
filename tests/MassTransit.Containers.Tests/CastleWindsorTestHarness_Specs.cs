namespace MassTransit.Containers.Tests
{
    using System.Threading.Tasks;
    using Castle.Windsor;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework.Messages;
    using Testing;


    [TestFixture]
    public class CastleWindsorTestHarness_Specs
    {
        [Test]
        public async Task Should_support_the_test_harness()
        {
            var container = new WindsorContainer()
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    cfg.AddConsumer<PingRequestConsumer>();
                });

            var harness = container.Resolve<InMemoryTestHarness>();

            await harness.Start();
            try
            {
                var bus = container.Resolve<IBus>();

                IRequestClient<PingMessage> client = bus.CreateRequestClient<PingMessage>();

                await client.GetResponse<PongMessage>(new PingMessage());

                Assert.That(await harness.Consumed.Any<PingMessage>());
            }
            finally
            {
                await harness.Stop();

                container.Dispose();
            }
        }
    }
}
