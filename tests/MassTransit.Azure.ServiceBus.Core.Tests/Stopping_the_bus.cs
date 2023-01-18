namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Testing;


    [TestFixture]
    public class Stopping_the_bus
    {
        [Test]
        [Explicit]
        public async Task Should_not_wait_forever_once_a_consumer_has_completed()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));

                    x.AddConsumer<SloRide>(c => c.UseConcurrentMessageLimit(1));

                    x.UsingTestAzureServiceBus((context, cfg) =>
                    {
                        cfg.PrefetchCount = 10;
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var consumerEndpoint = await harness.GetConsumerEndpoint<SloRide>();

            await consumerEndpoint.Send(new SloMessage());
            await consumerEndpoint.Send(new SloMessage());
            await consumerEndpoint.Send(new SloMessage());
            await consumerEndpoint.Send(new SloMessage());

            Assert.That(await harness.Consumed.Any<SloMessage>(), Is.True);

            await harness.Stop().OrTimeout(TimeSpan.FromSeconds(40));
        }


        class SloRide :
            IConsumer<SloMessage>
        {
            public async Task Consume(ConsumeContext<SloMessage> context)
            {
                await Task.Delay(TimeSpan.FromSeconds(8), context.CancellationToken);
            }
        }
    }


    public record SloMessage
    {
    }
}
