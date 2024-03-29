namespace MassTransit.Tests;

using System;
using System.Threading.Tasks;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestFramework.Messages;


[TestFixture]
public class SentTime_Specs
{
    [Test]
    public async Task Should_have_sent_time_header_in_utc()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddHandler(async (PingMessage _) =>
                {
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        await harness.Bus.Publish(new PingMessage());

        IReceivedMessage<PingMessage> consumed = await harness.Consumed.SelectAsync<PingMessage>().FirstOrDefault();
        Assert.That(consumed, Is.Not.Null);

        Assert.That(consumed.Context.SentTime.Value.Kind, Is.EqualTo(DateTimeKind.Utc));
    }
}
