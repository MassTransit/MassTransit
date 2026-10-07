#nullable enable
namespace MassTransit.Tests;

using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;


[TestFixture]
public class Setting_the_time_to_live_of_a_request
{
    [Test]
    public async Task Should_not_change_the_request_timeout()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<SlowStatusConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        IRequestClient<CheckStatus> client = harness.GetRequestClient<CheckStatus>();

        var timer = Stopwatch.StartNew();

        Assert.That(async () => await client.GetResponse<StatusChecked>(new CheckStatus(), x => x.TimeToLive = RequestTimeout.After(s: 10),
            timeout: RequestTimeout.After(ms: 500)), Throws.TypeOf<RequestTimeoutException>());

        Assert.That(timer.Elapsed, Is.LessThan(SlowStatusConsumer.Delay));
    }

    [Test]
    public async Task Should_send_the_request_without_a_time_to_live_when_cleared()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<StatusConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        IRequestClient<CheckStatus> client = harness.GetRequestClient<CheckStatus>();

        Response<StatusChecked> response = await client.GetResponse<StatusChecked>(new CheckStatus(), x => x.TimeToLive = RequestTimeout.None);

        Assert.That(response.Message, Is.Not.Null);

        IReceivedMessage<CheckStatus> received = harness.Consumed.Select<CheckStatus>().Single();

        Assert.That(received.Context.ExpirationTime, Is.Null);
    }
}


public record CheckStatus;


public record StatusChecked;


public class StatusConsumer :
    IConsumer<CheckStatus>
{
    public Task Consume(ConsumeContext<CheckStatus> context)
    {
        return context.RespondAsync(new StatusChecked());
    }
}


public class SlowStatusConsumer :
    IConsumer<CheckStatus>
{
    public static readonly System.TimeSpan Delay = System.TimeSpan.FromSeconds(2);

    public async Task Consume(ConsumeContext<CheckStatus> context)
    {
        await Task.Delay(Delay);

        await context.RespondAsync(new StatusChecked());
    }
}
