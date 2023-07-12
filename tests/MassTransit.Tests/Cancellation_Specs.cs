namespace MassTransit.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework.Messages;


    [TestFixture]
    public class When_the_transport_cancels
    {
        [Test]
        public async Task Should_not_produce_a_fault_on_shutdown()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddOptions<MassTransitHostOptions>()
                        .Configure(options =>
                        {
                            options.ConsumerStopTimeout = TimeSpan.FromSeconds(1);
                            options.StopTimeout = TimeSpan.FromSeconds(10);
                        });

                    x.AddTaskCompletionSource<PingMessage>();

                    x.AddHandler(async (ConsumeContext<PingMessage> context, TaskCompletionSource<PingMessage> source) =>
                    {
                        source.TrySetResult(context.Message);

                        await Task.Delay(5000, context.CancellationToken);
                    });
                }).BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish(new PingMessage());

            await provider.GetRequiredService<TaskCompletionSource<PingMessage>>().Task;

            await harness.Stop();
        }

        [Test]
        public async Task Should_produce_fault_on_timeout()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddOptions<MassTransitHostOptions>()
                        .Configure(options =>
                        {
                            options.ConsumerStopTimeout = TimeSpan.FromSeconds(1);
                            options.StopTimeout = TimeSpan.FromSeconds(10);
                        });

                    x.AddHandler(async (ConsumeContext<PingMessage> context) =>
                    {
                        await Task.Delay(5000, context.CancellationToken);
                    });

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UseTimeout(t => t.Timeout = TimeSpan.FromSeconds(1));

                        cfg.ConfigureEndpoints(context);
                    });
                }).BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish(new PingMessage());

            Assert.That(await harness.Published.Any<Fault<PingMessage>>(), Is.True);

            await harness.Stop();
        }

        [Test]
        public async Task Should_produce_fault_on_internal_cancellation()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddOptions<MassTransitHostOptions>()
                        .Configure(options =>
                        {
                            options.ConsumerStopTimeout = TimeSpan.FromSeconds(1);
                            options.StopTimeout = TimeSpan.FromSeconds(10);
                        });

                    x.AddHandler(async (ConsumeContext<PingMessage> context) =>
                    {
                        using var source = new CancellationTokenSource(TimeSpan.FromSeconds(1));

                        await Task.Delay(5000, source.Token);
                    });

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                }).BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            await harness.Start();

            await harness.Bus.Publish(new PingMessage());

            Assert.That(await harness.Published.Any<Fault<PingMessage>>(), Is.True);

            await harness.Stop();
        }
    }
}
