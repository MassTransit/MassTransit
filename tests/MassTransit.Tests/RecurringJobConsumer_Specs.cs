namespace MassTransit.Tests;

using System;
using System.Threading.Tasks;
using Contracts.JobService;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;


[TestFixture]
public class Configuring_a_recurring_job_consumer
{
    [Test]
    public async Task Should_support_configuration_of_the_job()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.AddConsumer<RecurringJobConsumer>();

                x.AddJobSagaStateMachines(options => options.SlotWaitTime = TimeSpan.FromSeconds(1));
                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(15));

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseDelayedMessageScheduler();

                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var client = harness.GetRequestClient<SubmitJob<RecurringJobMessage>>();

        var jobId = await client.AddOrUpdateRecurringJob(nameof(RecurringJobConsumer), new RecurringJobMessage(), x => x.Every(seconds: 5),
            harness.CancellationToken);

        Assert.That(await harness.Published.SelectAsync<JobCompleted<RecurringJobMessage>>().Take(2).Count(), Is.EqualTo(2));

        await harness.Stop();
    }

    [Test]
    public async Task Should_support_reconfiguration_of_the_job()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.AddConsumer<RecurringJobConsumer>();

                x.AddJobSagaStateMachines(options => options.SlotWaitTime = TimeSpan.FromSeconds(1));
                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(15), testTimeout: TimeSpan.FromSeconds(30));

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseDelayedMessageScheduler();

                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var client = harness.GetRequestClient<SubmitJob<RecurringJobMessage>>();

        var jobId = await client.AddOrUpdateRecurringJob(nameof(RecurringJobConsumer), new RecurringJobMessage(), "*/5 * * * * ?",
            harness.CancellationToken);

        Assert.That(await harness.Published.SelectAsync<JobCompleted<RecurringJobMessage>>().Take(1).Count(), Is.EqualTo(1));

        await client.AddOrUpdateRecurringJob(nameof(RecurringJobConsumer), new RecurringJobMessage(), "*/10 * * * * ?",
            harness.CancellationToken);

        Assert.That(await harness.Published.SelectAsync<JobCompleted<RecurringJobMessage>>().Take(2).Count(), Is.EqualTo(2));

        await harness.Stop();
    }

    [Test]
    public async Task Should_support_reconfiguration_of_the_job_with_no_changes()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.AddConsumer<RecurringJobConsumer>();

                x.AddJobSagaStateMachines(options => options.SlotWaitTime = TimeSpan.FromSeconds(1));
                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(15));

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseDelayedMessageScheduler();

                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var client = harness.GetRequestClient<SubmitJob<RecurringJobMessage>>();

        var jobId = await client.AddOrUpdateRecurringJob(nameof(RecurringJobConsumer), new RecurringJobMessage(), "*/5 * * * * ?",
            harness.CancellationToken);

        Assert.That(await harness.Published.SelectAsync<JobCompleted<RecurringJobMessage>>().Take(1).Count(), Is.EqualTo(1));

        await client.AddOrUpdateRecurringJob(nameof(RecurringJobConsumer), new RecurringJobMessage(), "*/5 * * * * ?",
            harness.CancellationToken);

        Assert.That(await harness.Published.SelectAsync<JobCompleted<RecurringJobMessage>>().Take(2).Count(), Is.EqualTo(2));

        await harness.Stop();
    }

    [Test]
    public async Task Should_support_scheduling_a_single_run_job()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.AddConsumer<RecurringJobConsumer>();

                x.AddJobSagaStateMachines(options => options.SlotWaitTime = TimeSpan.FromSeconds(1));
                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(15));

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseDelayedMessageScheduler();

                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        var client = harness.GetRequestClient<SubmitJob<RecurringJobMessage>>();

        var jobId = await client.ScheduleJob(DateTimeOffset.UtcNow.AddSeconds(3), new RecurringJobMessage(), harness.CancellationToken);

        Assert.That(await harness.Published.Any<JobCompleted<RecurringJobMessage>>(), Is.True);

        await harness.Stop();
    }


    public record RecurringJobConsumer :
        IJobConsumer<RecurringJobMessage>
    {
        readonly ILogger<RecurringJobConsumer> _logger;

        public RecurringJobConsumer(ILogger<RecurringJobConsumer> logger)
        {
            _logger = logger;
        }

        public Task Run(JobContext<RecurringJobMessage> context)
        {
            _logger.LogInformation("Every minute");

            return Task.CompletedTask;
        }
    }
}


public record RecurringJobMessage;
