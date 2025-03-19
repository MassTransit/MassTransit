#nullable enable
namespace MassTransit.RabbitMqTransport.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobService;
using MassTransit.Contracts.JobService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Testing;


[TestFixture]
public class JobDistributionStrategy_Specs
{
    [Test]
    public async Task Should_separate_jobs_by_region()
    {
        void ConfigureRabbitMq(IBusRegistrationConfigurator x)
        {
            x.SetJobConsumerOptions(options => options.HeartbeatInterval = TimeSpan.FromSeconds(5))
                .Endpoint(e =>
                {
                    e.AddConfigureEndpointCallback(cfg =>
                    {
                        if (cfg is IRabbitMqReceiveEndpointConfigurator rmq)
                            rmq.SetQuorumQueue();
                    });
                });

            x.SetKebabCaseEndpointNameFormatter();
            x.AddConfigureEndpointsCallback((_, _, cfg) =>
            {
                if (cfg is IRabbitMqReceiveEndpointConfigurator rmq)
                    rmq.SetQuorumQueue();
            });

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.SetQuorumQueue();
                cfg.UseDelayedMessageScheduler();

                cfg.ConfigureEndpoints(context);
            });
        }

        await using var provider = new ServiceCollection()
            .ConfigureRabbitMqTestOptions(options =>
            {
                options.CleanVirtualHost = true;
                options.CreateVirtualHostIfNotExists = true;
            })
            .AddMassTransitTestHarness(x =>
            {
                x.AddOptions<RabbitMqTransportOptions>()
                    .Configure(options => options.VHost = "test");

                x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));

                x.AddJobSagaStateMachines(options =>
                {
                    options.SlotWaitTime = TimeSpan.FromSeconds(3);
                });
                x.TryAddJobDistributionStrategy<RegionJobDistributionStrategy>();

                x.AddHandler<JobCompleted<RegionalJob>>()
                    .Endpoint(e => e.Name = "job-events");

                ConfigureRabbitMq(x);
            })
            .AddMassTransit<IEastRegionBus>(x =>
            {
                x.AddOptions<RabbitMqTransportOptions>(nameof(IEastRegionBus))
                    .Configure(options => options.VHost = "test");

                x.AddConsumer<RegionalJobConsumer>(c =>
                {
                    c.Options<JobOptions<RegionalJob>>(options =>
                        options
                            .SetJobTimeout(TimeSpan.FromSeconds(10))
                            .SetGlobalConcurrentJobLimit(1)
                            .SetJobTypeProperties(p => p.Set("Strategy", "Region"))
                            .SetInstanceProperties(p => p.Set("Region", "East"))
                    );
                });

                ConfigureRabbitMq(x);
            })
            .AddMassTransit<IWestRegionBus>(x =>
            {
                x.AddOptions<RabbitMqTransportOptions>(nameof(IWestRegionBus))
                    .Configure(options => options.VHost = "test");

                x.AddConsumer<RegionalJobConsumer>(c =>
                {
                    c.Options<JobOptions<RegionalJob>>(options =>
                        options
                            .SetJobTimeout(TimeSpan.FromSeconds(10))
                            .SetGlobalConcurrentJobLimit(1)
                            .SetJobTypeProperties(p => p.Set("Strategy", "Region"))
                            .SetInstanceProperties(p => p.Set("Region", "West"))
                    );
                });

                ConfigureRabbitMq(x);
            })
            .BuildServiceProvider(true);

        var harness = await provider.StartTestHarness();

        await harness.Bus.AddOrUpdateRecurringJob("EastRegion", new RegionalJob { Region = "East" }, x => x.CronExpression = "*/5 * * * * *",
            x => x.Set("Region", "East"));
        await harness.Bus.AddOrUpdateRecurringJob("WestRegion", new RegionalJob { Region = "West" }, x => x.CronExpression = "*/5 * * * * *",
            x => x.Set("Region", "West"));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(await harness.Consumed.Any<JobCompleted<RegionalJob>>(x => x.Context.Message.Job.Region == "West"));
            Assert.That(await harness.Consumed.Any<JobCompleted<RegionalJob>>(x => x.Context.Message.Job.Region == "East"));

            var completed = await harness.Consumed.SelectAsync<JobCompleted<RegionalJob>>(x => x.Context.Message.Job.Region == "East").FirstOrDefault();

            Assert.That(completed, Is.Not.Null);
            Assert.That(completed.Context.Message.JobProperties, Contains.Key("Region"));
            Assert.That(completed.Context.Message.InstanceProperties, Contains.Key("Region"));
        }

        await harness.Stop();
    }


    public class RegionJobDistributionStrategy :
        IJobDistributionStrategy
    {
        public Task<ActiveJob?> IsJobSlotAvailable(ConsumeContext<AllocateJobSlot> context, JobTypeInfo jobTypeInfo)
        {
            object? strategy = null;
            jobTypeInfo.Properties?.TryGetValue("Strategy", out strategy);

            return strategy switch
            {
                "Region" => IsRegionalJobSlotAvailable(context, jobTypeInfo),
                _ => DefaultJobDistributionStrategy.Instance.IsJobSlotAvailable(context, jobTypeInfo)
            };
        }

        static Task<ActiveJob?> IsRegionalJobSlotAvailable(ConsumeContext<AllocateJobSlot> context, JobTypeInfo jobTypeInfo)
        {
            var region = context.Message.JobProperties?.GetValueOrDefault("Region") as string;

            LogContext.Info?.Log("Allocating job slot for region: {Region}", region);

            var instances = from i in jobTypeInfo.Instances
                join a in jobTypeInfo.ActiveJobs on i.Key equals a.InstanceAddress into ai
                where (ai.Count() < jobTypeInfo.ConcurrentJobLimit && string.IsNullOrEmpty(region))
                    || ((i.Value.Properties?.TryGetValue("Region", out var r) ?? false) && r is string rs && rs == region)
                orderby ai.Count(), i.Value.Used
                select new
                {
                    Instance = i.Value,
                    InstanceAddress = i.Key,
                    InstanceCount = ai.Count()
                };

            var firstInstance = instances.FirstOrDefault();
            if (firstInstance == null)
                return Task.FromResult<ActiveJob?>(null);

            return Task.FromResult<ActiveJob?>(new ActiveJob
            {
                JobId = context.Message.JobId,
                InstanceAddress = firstInstance.InstanceAddress
            });
        }
    }


    class RegionalJobConsumer :
        IJobConsumer<RegionalJob>
    {
        readonly ILogger<RegionalJobConsumer> _logger;

        public RegionalJobConsumer(ILogger<RegionalJobConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Run(JobContext<RegionalJob> context)
        {
            _logger.LogInformation("Processing job for {Region} region", context.Job.Region);

            await Task.Delay(1000);
        }
    }


    public interface IEastRegionBus : IBus;


    public interface IWestRegionBus : IBus;


    public record RegionalJob
    {
        public string Region { get; init; }
    }
}
