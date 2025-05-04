namespace MassTransit.Tests;

using System;
using System.Threading.Tasks;
using InMemoryTransport;
using NUnit.Framework;


[TestFixture]
public class DelayProvider_Specs
{
    [Test]
    public async Task Should_delay_subsequent_delay()
    {
        await using var delayProvider = new InMemoryDelayProvider();

        var delayTask = delayProvider.Delay(1000);

        await delayProvider.Advance(TimeSpan.FromSeconds(1));

        await delayTask;

        var start = DateTime.UtcNow;

        await delayProvider.Delay(1000);

        var stop = DateTime.UtcNow;

        Console.WriteLine(stop - start);

        Assert.That(stop - start, Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(1)));
    }

    [Test]
    public async Task Should_manage_delays_in_order()
    {
        await using var delayProvider = new InMemoryDelayProvider();

        var start = DateTime.UtcNow;

        await delayProvider.Delay(1000);

        var stop = DateTime.UtcNow;

        Console.WriteLine(stop - start);

        Assert.That(stop - start, Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(1)));
    }

    [Test]
    public async Task Should_manage_delays_in_order_without_waiting()
    {
        await using var delayProvider = new InMemoryDelayProvider();

        var start = DateTime.UtcNow;

        var delayTask = delayProvider.Delay(1000);

        await delayProvider.Advance(TimeSpan.FromSeconds(1));

        await delayTask;

        var stop = DateTime.UtcNow;

        Console.WriteLine(stop - start);

        Assert.That(stop - start, Is.LessThan(TimeSpan.FromSeconds(0.5)));
    }

    [Test]
    public async Task Should_support_simultaneous_delays()
    {
        await using var delayProvider = new InMemoryDelayProvider();

        var readyTime = delayProvider.UtcNow.AddSeconds(1);

        var start = DateTime.UtcNow;

        var delayTask1 = delayProvider.Delay(readyTime);
        var delayTask2 = delayProvider.Delay(readyTime);

        await delayProvider.Advance(TimeSpan.FromSeconds(1));

        await Task.WhenAll(delayTask1, delayTask2);

        var stop = DateTime.UtcNow;

        Console.WriteLine(stop - start);

        Assert.That(stop - start, Is.LessThan(TimeSpan.FromSeconds(0.5)));
    }
}
