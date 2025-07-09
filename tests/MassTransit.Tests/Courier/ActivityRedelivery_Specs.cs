#nullable enable
namespace MassTransit.Tests.Courier;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit.Courier.Contracts;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;


[TestFixture]
public class Using_redelivery_with_an_activity
{
    [Test]
    public async Task Should_include_all_variables([Values(1, 2, 3)] int redeliveryCount)
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddConsumer<ClaimRewardEventConsumer>();
                x.AddExecuteActivity<ValidateClaimedRewardActivity, ValidateClaimedRewardArguments>();

                x.AddConfigureEndpointsCallback((context, name, cfg) =>
                {
                    cfg.UseDelayedRedelivery(r => r.Interval(redeliveryCount, 100));
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        RoutingSlipBuilder builder = new(NewId.NextGuid());

        var submissionId = NewId.NextGuid();
        var customerGuid = NewId.NextGuid().ToString();
        var rewardId = NewId.NextGuid().ToString();

        builder.SetVariables(new
        {
            submissionId,
            customerGuid,
            rewardId
        });

        builder.AddActivity(nameof(ValidateClaimedRewardActivity),
            harness.GetExecuteActivityAddress<ValidateClaimedRewardActivity, ValidateClaimedRewardArguments>());

        await builder.AddSubscription(harness.GetConsumerAddress<ClaimRewardEventConsumer>(),
            RoutingSlipEvents.ActivityFaulted,
            RoutingSlipEventContents.Variables,
            nameof(ValidateClaimedRewardActivity),
            x => x.Send<ValidateClaimedRewardFailed>(new { submissionId }));

        await harness.Bus.Execute(builder.Build());

        Assert.That(await harness.Consumed.Any<ValidateClaimedRewardFailed>(), Is.True);

        IReceivedMessage<ValidateClaimedRewardFailed> context = await harness.Consumed.SelectAsync<ValidateClaimedRewardFailed>().FirstOrDefault();
        Assert.That(context, Is.Not.Null);

        Assert.That(context.Context.Message.SubmissionId, Is.EqualTo(submissionId));
        Assert.That(context.Context.Message.ErrorMessage, Is.EqualTo("UglyError"));
    }
}


public sealed record ValidateClaimedRewardFailed :
    ContractWithVariables
{
    public Guid SubmissionId { get; set; }

    public string? ErrorMessage => GetVariableData<string?>(nameof(ErrorMessage));

    public ExceptionInfo? ExceptionInfo { get; set; }
}


public class ClaimRewardEventConsumer :
    IConsumer<ValidateClaimedRewardFailed>
{
    public Task Consume(ConsumeContext<ValidateClaimedRewardFailed> context)
    {
        return Task.CompletedTask;
    }
}


public abstract record ContractWithVariables
{
    public Dictionary<string, object>? Variables { get; set; }

    protected T? GetVariableData<T>(string key)
    {
        if (Variables is not null && Variables.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        return default;
    }
}


public class ValidateClaimedRewardActivity :
    IExecuteActivity<ValidateClaimedRewardArguments>
{
    public async Task<ExecutionResult> Execute(ExecuteContext<ValidateClaimedRewardArguments> context)
    {
        await Task.Delay(1);

        var errorMessage = "UglyError";
        return context.FaultedWithVariables(new ClaimRewardException(errorMessage), new { ErrorMessage = errorMessage });
    }
}


public sealed class ValidateClaimedRewardArguments
{
    public Guid SubmissionId { get; set; }

    public string CustomerGuid { get; set; }

    public string RewardId { get; set; }
}


public sealed class ClaimRewardException :
    Exception
{
    public ClaimRewardException()
    {
    }

    public ClaimRewardException(string message)
        : base(message)
    {
    }

    public ClaimRewardException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
