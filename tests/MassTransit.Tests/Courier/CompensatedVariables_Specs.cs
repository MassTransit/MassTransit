#nullable enable
namespace MassTransit.Tests.Courier;

using System.Linq;
using System.Threading.Tasks;
using MassTransit.Courier.Contracts;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TestFramework.Courier;


[TestFixture]
public class When_a_compensation_sets_variables
{
    [Test]
    public async Task Should_pass_them_to_the_next_compensation()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddActivity<BookFlightActivity, BookFlightArguments, BookFlightLog>();
                x.AddActivity<ChargeCardActivity, ChargeCardArguments, ChargeCardLog>();
                x.AddActivity<FaultyActivity, FaultyArguments, FaultyLog>();
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var builder = new RoutingSlipBuilder(NewId.NextGuid());
        builder.AddActivity(nameof(BookFlightActivity), harness.GetExecuteActivityAddress<BookFlightActivity, BookFlightArguments>());
        builder.AddActivity(nameof(ChargeCardActivity), harness.GetExecuteActivityAddress<ChargeCardActivity, ChargeCardArguments>());
        builder.AddActivity(nameof(FaultyActivity), harness.GetExecuteActivityAddress<FaultyActivity, FaultyArguments>());

        await harness.Bus.Execute(builder.Build());

        Assert.That(await harness.Published.Any<RoutingSlipActivityCompensated>(x => x.Context.Message.ActivityName == nameof(BookFlightActivity)),
            Is.True);

        var compensated = harness.Published.Select<RoutingSlipActivityCompensated>()
            .Single(x => x.Context.Message.ActivityName == nameof(BookFlightActivity));

        Assert.That(compensated.Context.Message.Variables.TryGetValue("RefundId", out var refundId), Is.True);
        Assert.That(refundId?.ToString(), Is.EqualTo("R-42"));
    }
}


public class BookFlightActivity :
    IActivity<BookFlightArguments, BookFlightLog>
{
    public Task<ExecutionResult> Execute(ExecuteContext<BookFlightArguments> context)
    {
        return Task.FromResult(context.Completed<BookFlightLog>(new { BookingId = "B-7" }));
    }

    public Task<CompensationResult> Compensate(CompensateContext<BookFlightLog> context)
    {
        return Task.FromResult(context.Compensated());
    }
}


public sealed class BookFlightArguments
{
}


public sealed class BookFlightLog
{
    public string? BookingId { get; set; }
}


public class ChargeCardActivity :
    IActivity<ChargeCardArguments, ChargeCardLog>
{
    public Task<ExecutionResult> Execute(ExecuteContext<ChargeCardArguments> context)
    {
        return Task.FromResult(context.Completed<ChargeCardLog>(new { ChargeId = "C-9" }));
    }

    public Task<CompensationResult> Compensate(CompensateContext<ChargeCardLog> context)
    {
        return Task.FromResult(context.Compensated(new { RefundId = "R-42" }));
    }
}


public sealed class ChargeCardArguments
{
}


public sealed class ChargeCardLog
{
    public string? ChargeId { get; set; }
}
