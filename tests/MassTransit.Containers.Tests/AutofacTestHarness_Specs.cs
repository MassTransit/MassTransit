namespace MassTransit.Containers.Tests
{
    using System.Threading.Tasks;
    using Autofac;
    using AutofacIntegration;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework.Messages;
    using Testing;


    [TestFixture]
    public class AutofacTestHarness_Specs
    {
        [Test]
        public async Task Should_support_the_test_harness()
        {
            var provider = new ContainerBuilder()
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    cfg.AddConsumer<PingRequestConsumer>();
                })
                .Build();

            var harness = provider.Resolve<InMemoryTestHarness>();

            await harness.Start();
            try
            {
                var bus = provider.Resolve<IBus>();

                IRequestClient<PingMessage> client = bus.CreateRequestClient<PingMessage>();

                await client.GetResponse<PongMessage>(new PingMessage());

                Assert.That(await harness.Consumed.Any<PingMessage>());
            }
            finally
            {
                await harness.Stop();

                await provider.DisposeAsync();
            }
        }
    }
}
