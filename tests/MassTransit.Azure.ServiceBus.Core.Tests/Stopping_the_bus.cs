namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
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

        [Test]
        [Explicit]
        public async Task Should_publish_a_message_after_being_stopped()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10), testTimeout: TimeSpan.FromSeconds(60));

                    x.AddConsumer<SloRide>(c => c.UseConcurrentMessageLimit(1));

                    x.UsingTestAzureServiceBus((context, cfg) =>
                    {
                        cfg.PrefetchCount = 10;
                    });

                    x.AddOptions<MassTransitHostOptions>().Configure(options =>
                    {
                        options.WaitUntilStarted = true;
                        options.StartTimeout = TimeSpan.FromSeconds(10);
                        options.StopTimeout = TimeSpan.FromSeconds(30);
                        options.ConsumerStopTimeout = TimeSpan.FromSeconds(10);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            var consumerEndpoint = await harness.GetConsumerEndpoint<SloRide>();

            await consumerEndpoint.Send(new SloMessage());

            await Task.Delay(4000);

            await harness.Stop();

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await harness.Consumed.Any<SloMessage>(), Is.True);
                Assert.That(await harness.Published.Any<SloResult>(), Is.True);
            });
        }


        class SloRide :
            IConsumer<SloMessage>
        {
            readonly ILogger<SloRide> _logger;

            public SloRide(ILogger<SloRide> logger)
            {
                _logger = logger;
            }

            public async Task Consume(ConsumeContext<SloMessage> context)
            {
                _logger.LogInformation("Starting the slow ride");

                await Task.Delay(TimeSpan.FromSeconds(8), context.CancellationToken);

                await context.Publish(new SloResult());

                _logger.LogInformation("Finished the slow ride");
            }
        }
    }


    public record SloMessage
    {
    }


    public record SloResult
    {
    }
}
