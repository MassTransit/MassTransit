#nullable enable
namespace MassTransit.Tests.Courier;

using System.Threading.Tasks;
using MassTransit.Courier.Contracts;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestFramework.Courier;


[TestFixture]
public class When_an_activity_completes_with_an_empty_log
{
    [Test]
    public async Task Should_be_compensated_when_a_later_activity_faults()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddActivity<ReserveSeatActivity, ReserveSeatArguments, ReserveSeatLog>();
                x.AddActivity<FaultyActivity, FaultyArguments, FaultyLog>();
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var builder = new RoutingSlipBuilder(NewId.NextGuid());
        builder.AddActivity(nameof(ReserveSeatActivity), harness.GetExecuteActivityAddress<ReserveSeatActivity, ReserveSeatArguments>());
        builder.AddActivity(nameof(FaultyActivity), harness.GetExecuteActivityAddress<FaultyActivity, FaultyArguments>());

        await harness.Bus.Execute(builder.Build());

        Assert.That(await harness.Published.Any<RoutingSlipFaulted>(), Is.True);

        Assert.That(await harness.Published.Any<RoutingSlipActivityCompensated>(x => x.Context.Message.ActivityName == nameof(ReserveSeatActivity)),
            Is.True);
    }
}


public class ReserveSeatActivity :
    IActivity<ReserveSeatArguments, ReserveSeatLog>
{
    public Task<ExecutionResult> Execute(ExecuteContext<ReserveSeatArguments> context)
    {
        return Task.FromResult(context.Completed<ReserveSeatLog>(new { }));
    }

    public Task<CompensationResult> Compensate(CompensateContext<ReserveSeatLog> context)
    {
        return Task.FromResult(context.Compensated());
    }
}


public sealed class ReserveSeatArguments
{
}


/// <summary>
/// Nothing needs to be logged to compensate this activity
/// </summary>
public sealed class ReserveSeatLog
{
}
