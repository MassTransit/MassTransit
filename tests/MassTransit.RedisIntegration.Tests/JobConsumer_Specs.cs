namespace MassTransit.RedisIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Contracts.JobService;
    using JobConsumerTests;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Testing;


    namespace JobConsumerTests
    {
        using System;
        using System.Threading.Tasks;
        using Contracts.JobService;


        public interface OddJob
        {
            TimeSpan Duration { get; }
        }


        public class OddJobConsumer :
            IJobConsumer<OddJob>
        {
            public async Task Run(JobContext<OddJob> context)
            {
                if (context.RetryAttempt == 0)
                    await Task.Delay(context.Job.Duration, context.CancellationToken);
            }
        }


        public class OddJobCompletedConsumer :
            IConsumer<JobCompleted<OddJob>>
        {
            public Task Consume(ConsumeContext<JobCompleted<OddJob>> context)
            {
                return Task.CompletedTask;
            }
        }
    }


    [TestFixture]
    public class Using_the_new_job_service_configuration
    {
        [Test]
        public async Task Should_complete_the_job()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));

                    x.SetKebabCaseEndpointNameFormatter();

                    x.AddConsumer<OddJobConsumer>()
                        .Endpoint(e => e.Name = "odd-job");

                    x.AddConsumer<OddJobCompletedConsumer>()
                        .Endpoint(e => e.ConcurrentMessageLimit = 1);

                    x.SetJobConsumerOptions(options => options.HeartbeatInterval = TimeSpan.FromSeconds(10))
                        .Endpoint(e => e.PrefetchCount = 100);

                    x.AddJobSagaStateMachines()
                        .RedisRepository();

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
                var jobId = NewId.NextGuid();

                IRequestClient<SubmitJob<OddJob>> client = harness.GetRequestClient<SubmitJob<OddJob>>();

                Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new
                {
                    JobId = jobId,
                    Job = new { Duration = TimeSpan.FromSeconds(1) }
                });

                await Assert.MultipleAsync(async () =>
                {
                    Assert.That(response.Message.JobId, Is.EqualTo(jobId));

                    Assert.That(await harness.Published.Any<JobSubmitted>(), Is.True);
                    Assert.That(await harness.Published.Any<JobStarted>(), Is.True);

                    Assert.That(await harness.Published.Any<JobCompleted>(), Is.True);
                    Assert.That(await harness.Published.Any<JobCompleted<OddJob>>(), Is.True);
                });
            }
            finally
            {
                await harness.Stop();
            }
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
        }
    }
}
