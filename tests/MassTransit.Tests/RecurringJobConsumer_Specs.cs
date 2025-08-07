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
    public async Task Should_support_cancellation_and_continuation_of_the_job()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.AddConsumer<RecurringJobConsumer>();

                x.AddJobSagaStateMachines(options => options.SlotWaitTime = TimeSpan.FromSeconds(1));
                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseDelayedMessageScheduler();

                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();
        try
        {
            IRequestClient<SubmitJob<RecurringJobMessage>> client = harness.GetRequestClient<SubmitJob<RecurringJobMessage>>();

            var jobId = await client.AddOrUpdateRecurringJob(nameof(RecurringJobConsumer), new RecurringJobMessage(), x => x.Every(seconds: 5),
                harness.CancellationToken);

            Assert.That(await harness.Published.SelectAsync<JobCompleted<RecurringJobMessage>>().Take(1).Count(), Is.EqualTo(1));

            await harness.Bus.CancelRecurringJob<RecurringJobMessage>(nameof(RecurringJobConsumer), "Not right now");

            Assert.That(await harness.Published.SelectAsync<JobCanceled>(x => x.Context.Message.JobId == jobId).Take(1).Count(), Is.EqualTo(1));

            await client.AddOrUpdateRecurringJob(nameof(RecurringJobConsumer), new RecurringJobMessage(), x => x.Every(seconds: 5),
                harness.CancellationToken);

            Assert.That(await harness.Published.SelectAsync<JobCompleted<RecurringJobMessage>>().Take(2).Count(), Is.EqualTo(2));
        }
        finally
        {
            await harness.Stop();
        }
    }

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

        IRequestClient<SubmitJob<RecurringJobMessage>> client = harness.GetRequestClient<SubmitJob<RecurringJobMessage>>();

        var jobId = await client.AddOrUpdateRecurringJob(nameof(RecurringJobConsumer), new RecurringJobMessage(), x => x.Every(seconds: 5),
            harness.CancellationToken);

        Assert.That(await harness.Published.SelectAsync<JobCompleted<RecurringJobMessage>>().Take(2).Count(), Is.EqualTo(2));

        await harness.Stop();
    }

    [Test]
    [Explicit]
    public async Task Should_support_multiple_jobs_of_the_same_type_with_different_names()
    {
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();

                x.AddConsumer<MaintenanceJobConsumer>();

                x.AddJobSagaStateMachines(options => options.SlotWaitTime = TimeSpan.FromSeconds(1));
                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(15), testTimeout: TimeSpan.FromMinutes(5));

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.UseDelayedMessageScheduler();

                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        IRequestClient<SubmitJob<MaintenanceTask>> client = harness.GetRequestClient<SubmitJob<MaintenanceTask>>();

        var job1Id = await client.AddOrUpdateRecurringJob("One", new MaintenanceTask { Name = "One" }, x => x.Every(seconds: 4), harness.CancellationToken);
        var job2Id = await client.AddOrUpdateRecurringJob("Two", new MaintenanceTask { Name = "Two" }, x => x.Every(seconds: 8), harness.CancellationToken);
        var job3Id = await client.AddOrUpdateRecurringJob("Tree", new MaintenanceTask { Name = "Tree" }, x => x.Every(seconds: 10), harness.CancellationToken);
        var job4Id = await client.AddOrUpdateRecurringJob("Four", new MaintenanceTask { Name = "Four" }, x => x.Every(seconds: 15), harness.CancellationToken);

        Assert.That(await harness.Published.SelectAsync<JobCompleted<MaintenanceTask>>().Take(30).Count(), Is.EqualTo(30));

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

        IRequestClient<SubmitJob<RecurringJobMessage>> client = harness.GetRequestClient<SubmitJob<RecurringJobMessage>>();

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

        IRequestClient<SubmitJob<RecurringJobMessage>> client = harness.GetRequestClient<SubmitJob<RecurringJobMessage>>();

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

        IRequestClient<SubmitJob<RecurringJobMessage>> client = harness.GetRequestClient<SubmitJob<RecurringJobMessage>>();

        var jobId = await client.ScheduleJob(DateTimeOffset.UtcNow.AddSeconds(3), new RecurringJobMessage(), harness.CancellationToken);

        Assert.That(await harness.Published.Any<JobCompleted<RecurringJobMessage>>(), Is.True);

        await harness.Stop();
    }


    public class RecurringJobConsumer :
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


    public class MaintenanceJobConsumer :
        IJobConsumer<MaintenanceTask>
    {
        readonly ILogger<MaintenanceJobConsumer> _logger;

        public MaintenanceJobConsumer(ILogger<MaintenanceJobConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Run(JobContext<MaintenanceTask> context)
        {
            _logger.LogInformation("Running MaintenanceTask: {Id} {Name}", context.JobId, context.Job.Name);

            try
            {
                await Task.Delay(4000, context.CancellationToken);

                _logger.LogInformation("MaintenanceTask completed: {Id} {Name}", context.JobId, context.Job.Name);
            }
            catch (OperationCanceledException) when (context.CancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("MaintenanceTask exception: {Id} {Name}", context.JobId, context.Job.Name);

                await context.SaveJobState(new MaintenanceTaskState { Variance = 10000 });

                throw;
            }
        }
    }
}


public record RecurringJobMessage;


public record MaintenanceTask
{
    public string Name { get; set; }
}


public record MaintenanceTaskState
{
    public int Variance { get; set; }
}
