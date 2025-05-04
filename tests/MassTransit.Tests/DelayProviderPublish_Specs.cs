namespace MassTransit.Tests;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using InMemoryTransport;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;


[TestFixture]
public class DelayProviderPublish_Specs
{
    [Test]
    public async Task Should_advance_the_delay_provider()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<DelayConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();


        var scheduler = harness.Scope.ServiceProvider.GetRequiredService<IMessageScheduler>();

        var timer = Stopwatch.StartNew();

        await scheduler.SchedulePublish(TimeSpan.FromSeconds(10), new DelayPublishAdvance());

        var delayProvider = harness.Scope.ServiceProvider.GetRequiredService<IInMemoryDelayProvider>();
        await delayProvider.Advance(TimeSpan.FromSeconds(10));

        await Assert.MultipleAsync(async () =>
        {
            Assert.That(await harness.Consumed.Any<DelayPublishAdvance>(), Is.True);

            timer.Stop();

            Assert.That(timer.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
        });
    }


    public record DelayPublishAdvance;


    class DelayConsumer :
        IConsumer<DelayPublishAdvance>
    {
        public Task Consume(ConsumeContext<DelayPublishAdvance> context)
        {
            return Task.CompletedTask;
        }
    }
}
