namespace MassTransit.Tests.ContainerTests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using TestFramework.Messages;


    [TestFixture]
    public class Stop_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_respect_ConsumerStopTimeout()
        {
            var collection = new ServiceCollection();
            collection.AddOptions<MassTransitHostOptions>().Configure(options => options.ConsumerStopTimeout = TimeSpan.FromSeconds(1));
            collection.AddMassTransitTestHarness(configurator =>
            {
                configurator.AddTaskCompletionSource<ConsumeContext<PingMessage>>();
                configurator.AddConsumer<StuckConsumer>();
            });

            await using var provider = collection.BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            await harness.Start();

            await harness.Bus.Publish(new PingMessage(), harness.CancellationToken);

            await provider.GetTask<ConsumeContext<PingMessage>>();

            await harness.Stop(harness.CancellationToken);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            Assert.That(await harness.Consumed.Any<PingMessage>(x => x.Exception is OperationCanceledException, cts.Token), Is.True);
        }


        class StuckConsumer :
            IConsumer<PingMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<PingMessage>> _taskCompletionSource;

            public StuckConsumer(TaskCompletionSource<ConsumeContext<PingMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Consume(ConsumeContext<PingMessage> context)
            {
                _taskCompletionSource.TrySetResult(context);
                return Task.Delay(TimeSpan.FromMinutes(10), context.CancellationToken);
            }
        }
    }


    [TestFixture]
    public class Stop_Retry_Specs :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_respect_ConsumerStopTimeout()
        {
            var collection = new ServiceCollection();
            collection.AddOptions<MassTransitHostOptions>().Configure(options => options.ConsumerStopTimeout = TimeSpan.FromSeconds(1));
            collection.AddMassTransitTestHarness(configurator =>
            {
                configurator.AddTaskCompletionSource<ConsumeContext<PingMessage>>();
                configurator.AddConsumer<StuckConsumer, StuckConsumerDefinition>();
            });

            await using var provider = collection.BuildServiceProvider(true);

            var harness = provider.GetTestHarness();
            await harness.Start();

            await harness.Bus.Publish(new PingMessage(), harness.CancellationToken);

            await provider.GetTask<ConsumeContext<PingMessage>>();

            await harness.Stop(harness.CancellationToken);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            Assert.That(await harness.Consumed.Any<PingMessage>(x => x.Exception is OperationCanceledException, cts.Token), Is.True);
        }


        class StuckConsumer :
            IConsumer<PingMessage>
        {
            readonly TaskCompletionSource<ConsumeContext<PingMessage>> _taskCompletionSource;

            public StuckConsumer(TaskCompletionSource<ConsumeContext<PingMessage>> taskCompletionSource)
            {
                _taskCompletionSource = taskCompletionSource;
            }

            public Task Consume(ConsumeContext<PingMessage> context)
            {
                try
                {
                    throw new ArgumentException("Expected error, causing retries");
                }
                finally
                {
                    _taskCompletionSource.TrySetResult(context);
                }
            }
        }


        class StuckConsumerDefinition :
            ConsumerDefinition<StuckConsumer>
        {
            protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
                IConsumerConfigurator<StuckConsumer> consumerConfigurator,
                IRegistrationContext context)
            {
                endpointConfigurator.UseMessageRetry(r => r.Interval(10, TimeSpan.FromMinutes(1)));
            }
        }
    }
}
