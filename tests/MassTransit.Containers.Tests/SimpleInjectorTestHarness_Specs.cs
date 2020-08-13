namespace MassTransit.Containers.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Scenarios;
    using SimpleInjector;
    using SimpleInjector.Lifestyles;
    using TestFramework.Messages;
    using Testing;


    [TestFixture]
    public class SimpleInjectorTestHarness_Specs
    {
        [Test]
        public async Task Should_support_the_test_harness()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            container.AddMassTransitInMemoryTestHarness(cfg =>
            {
                cfg.AddConsumer<PingRequestConsumer>();
            });

            var harness = container.GetInstance<InMemoryTestHarness>();

            await harness.Start();
            try
            {
                var bus = container.GetInstance<IBus>();

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
