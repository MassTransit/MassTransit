namespace MassTransit.Containers.Tests
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Scenarios;
    using TestFramework.Messages;
    using Testing;


    [TestFixture]
    public class DependencyInjectionTestHarness_Specs
    {
        [Test]
        public async Task Should_support_the_test_harness()
        {
            var provider = new ServiceCollection()
                .AddInMemoryTestHarness(cfg =>
                {
                    cfg.AddConsumer<PingRequestConsumer>();
                })
                .BuildServiceProvider(true);

            var harness = provider.GetRequiredService<InMemoryTestHarness>();

            await harness.Start();
            try
            {
                var bus = provider.GetRequiredService<IBus>();

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
