namespace MassTransit.Tests
{
    using System;
    using System.Threading.Tasks;
    using Contracts.JobService;
    using JobConsumerTests;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


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
                if (context.TryGetJobState<OddJobState>(out var previousState))
                {
                    LogContext.Debug?.Log("Previous AttemptId: {LastAttemptId}", previousState.LastAttemptId);
                }

                try
                {
                    await context.SetJobProgress(0, 100);

                    if (context.RetryAttempt == 0)
                        await Task.Delay(context.Job.Duration, context.CancellationToken);

                    if (context.RetryAttempt > 0 && context.LastProgressValue is null)
                    {
                        throw new InvalidOperationException("The progress was not stored");
                    }

                    for (int i = 0; i < 100; i++)
                    {
                        await context.SetJobProgress(i, 100);
                    }
                }
                catch (OperationCanceledException)
                {
                    await context.SaveJobState(new OddJobState { LastAttemptId = context.AttemptId });
                    throw;
                }
            }
        }


        public class OddJobState
        {
            public Guid LastAttemptId { get; set; }
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

                    x.SetInMemorySagaRepositoryProvider();

                    x.AddJobSagaStateMachines();
                    x.SetJobConsumerOptions(options => options.HeartbeatInterval = TimeSpan.FromSeconds(10))
                        .Endpoint(e => e.PrefetchCount = 100);

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

                var responseJobId = await client.SubmitJob(jobId, new { Duration = TimeSpan.FromSeconds(1) });

                await Assert.MultipleAsync(async () =>
                {
                    Assert.That(responseJobId, Is.EqualTo(jobId));

                    Assert.That(await harness.Published.Any<JobSubmitted>(), Is.True);

                    Assert.That(await harness.Published.Any<JobStarted>(), Is.True);
                    Assert.That(await harness.Published.Any<JobStarted<OddJob>>(), Is.True);

                    Assert.That(await harness.Published.Any<JobCompleted>(), Is.True);
                    Assert.That(await harness.Published.Any<JobCompleted<OddJob>>(), Is.True);
                });
            }
            finally
            {
                await harness.Stop();
            }
        }
    }


    [TestFixture]
    public class Using_outbound_scoped_filters_with_the_job_service
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

                    x.SetInMemorySagaRepositoryProvider();

                    x.AddJobSagaStateMachines();
                    x.SetJobConsumerOptions(options => options.HeartbeatInterval = TimeSpan.FromSeconds(10))
                        .Endpoint(e => e.PrefetchCount = 100);

                    x.AddConfigureEndpointsCallback((context, name, cfg) =>
                    {
                        cfg.UsePublishFilter(typeof(JobTestPublishFilter<>), context);
                    });

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

                var responseJobId = await client.SubmitJob(jobId, new { Duration = TimeSpan.FromSeconds(1) });

                await Assert.MultipleAsync(async () =>
                {
                    Assert.That(responseJobId, Is.EqualTo(jobId));

                    Assert.That(await harness.Published.Any<JobSubmitted>(), Is.True);

                    Assert.That(await harness.Published.Any<JobStarted>(), Is.True);
                    Assert.That(await harness.Published.Any<JobStarted<OddJob>>(), Is.True);

                    Assert.That(await harness.Published.Any<JobCompleted>(), Is.True);
                    Assert.That(await harness.Published.Any<JobCompleted<OddJob>>(), Is.True);
                });
            }
            finally
            {
                await harness.Stop();
            }
        }


        public class JobTestPublishFilter<T> :
            IFilter<PublishContext<T>>
            where T : class
        {
            public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
            {
                return next.Send(context);
            }

            public void Probe(ProbeContext context)
            {
            }
        }
    }


    [TestFixture]
    public class JobConsumer_Specs
    {
        [Test]
        public async Task Should_cancel_the_job()
        {
            await using var provider = SetupServiceCollection();

            var harness = provider.GetTestHarness();

            await harness.Start();

            var jobId = NewId.NextGuid();

            IRequestClient<SubmitJob<OddJob>> client = harness.GetRequestClient<SubmitJob<OddJob>>();

            var responseJobId = await client.SubmitJob(jobId, new { Duration = TimeSpan.FromSeconds(10) });

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(responseJobId, Is.EqualTo(jobId));

                Assert.That(await harness.Published.Any<JobSubmitted>(), Is.True);

                Assert.That(await harness.Published.Any<JobStarted>(), Is.True);
                Assert.That(await harness.Published.Any<JobStarted<OddJob>>(), Is.True);
            });

            await harness.Bus.CancelJob(jobId);

            Assert.That(await harness.Published.Any<JobCanceled>(), Is.True);

            await harness.Stop();
        }

        [Test]
        public async Task Should_cancel_the_job_and_get_the_status()
        {
            await using var provider = SetupServiceCollection();

            var harness = provider.GetTestHarness();

            await harness.Start();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(10);

            var jobId = NewId.NextGuid();

            IRequestClient<SubmitJob<OddJob>> client = harness.GetRequestClient<SubmitJob<OddJob>>();

            var responseJobId = await client.SubmitJob(jobId, new { Duration = TimeSpan.FromSeconds(10) });

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(responseJobId, Is.EqualTo(jobId));

                Assert.That(await harness.Published.Any<JobSubmitted>(), Is.True);

                Assert.That(await harness.Published.Any<JobStarted>(), Is.True);
                Assert.That(await harness.Published.Any<JobStarted<OddJob>>(), Is.True);
            });

            IRequestClient<GetJobState> stateClient = harness.GetRequestClient<GetJobState>();

            Response<JobState> jobState = await stateClient.GetResponse<JobState>(new { JobId = jobId });

            Assert.That(jobState.Message.CurrentState, Is.EqualTo("Started"));

            await harness.Bus.CancelJob(jobId);

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await harness.Published.Any<JobCanceled>(), Is.True);
                Assert.That(await harness.Sent.Any<JobSlotReleased>(), Is.True);
            });

            jobState = await stateClient.GetResponse<JobState>(new { JobId = jobId });

            Assert.That(jobState.Message.CurrentState, Is.EqualTo("Canceled"));

            await harness.Stop();
        }

        [Test]
        public async Task Should_cancel_the_job_and_retry_it()
        {
            await using var provider = SetupServiceCollection();

            var harness = provider.GetTestHarness();

            await harness.Start();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(5);

            var jobId = NewId.NextGuid();

            IRequestClient<SubmitJob<OddJob>> client = harness.GetRequestClient<SubmitJob<OddJob>>();

            var responseJobId = await client.SubmitJob(jobId, new { Duration = TimeSpan.FromSeconds(10) });

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(responseJobId, Is.EqualTo(jobId));

                Assert.That(await harness.Published.Any<JobSubmitted>(), Is.True);

                Assert.That(await harness.Published.Any<JobStarted>(), Is.True);
                Assert.That(await harness.Published.Any<JobStarted<OddJob>>(), Is.True);
            });

            await harness.Bus.CancelJob(jobId);

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await harness.Published.Any<JobCanceled>(), Is.True);
                Assert.That(await harness.Sent.Any<JobSlotReleased>(), Is.True);
            });

            await harness.Bus.RetryJob(jobId);

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await harness.Published.Any<JobCompleted>(), Is.True);
                Assert.That(await harness.Published.Any<JobCompleted<OddJob>>(), Is.True);
            });

            await harness.Stop();
        }

        [Test]
        public async Task Should_cancel_the_job_while_waiting()
        {
            await using var provider = SetupServiceCollection();

            var harness = provider.GetTestHarness();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(10);

            await harness.Start();

            var previousJobId = NewId.NextGuid();

            var jobId = NewId.NextGuid();

            IRequestClient<SubmitJob<OddJob>> client = harness.GetRequestClient<SubmitJob<OddJob>>();

            await client.SubmitJob(previousJobId, new { Duration = TimeSpan.FromSeconds(10) });

            Assert.That(await harness.Published.Any<JobStarted>(x => x.Context.Message.JobId == previousJobId), Is.True);

            var responseJobId = await client.SubmitJob(jobId, new { Duration = TimeSpan.FromSeconds(10) });

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await harness.Published.Any<JobSubmitted>(x => x.Context.Message.JobId == jobId), Is.True);

                Assert.That(responseJobId, Is.EqualTo(jobId));

                Assert.That(await harness.Sent.Any<JobSlotWaitElapsed>(x => x.Context.Message.JobId == jobId), Is.True);
            });

            await harness.Bus.CancelJob(jobId);

            Assert.That(await harness.Published.Any<JobCanceled>(x => x.Context.Message.JobId == jobId), Is.True);

            await harness.Bus.CancelJob(previousJobId);

            Assert.That(await harness.Published.Any<JobCanceled>(x => x.Context.Message.JobId == previousJobId), Is.True);

            await harness.Stop();
        }

        [Test]
        public async Task Should_complete_the_job()
        {
            await using var provider = SetupServiceCollection();

            var harness = provider.GetTestHarness();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(5);

            await harness.Start();

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
                Assert.That(await harness.Published.Any<JobStarted<OddJob>>(), Is.True);

                Assert.That(await harness.Published.Any<JobCompleted>(), Is.True);
                Assert.That(await harness.Published.Any<JobCompleted<OddJob>>(), Is.True);
            });

            await harness.Stop();
        }

        [Test]
        public async Task Should_create_a_unique_job_id()
        {
            await using var provider = SetupServiceCollection();

            var harness = provider.GetTestHarness();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(5);

            await harness.Start();

            await harness.Bus.Publish<OddJob>(new { Duration = TimeSpan.FromSeconds(1) });

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await harness.Published.Any<JobSubmitted>(), Is.True);

                Assert.That(await harness.Published.Any<JobStarted>(), Is.True);
                Assert.That(await harness.Published.Any<JobStarted<OddJob>>(), Is.True);

                Assert.That(await harness.Published.Any<JobCompleted>(), Is.True);
                Assert.That(await harness.Published.Any<JobCompleted<OddJob>>(), Is.True);
            });

            IPublishedMessage<JobCompleted> publishedMessage = await harness.Published.SelectAsync<JobCompleted>().First();
            Assert.That(publishedMessage.Context.Message.JobId, Is.Not.EqualTo(Guid.Empty));

            await harness.Stop();
        }

        [Test]
        public async Task Should_return_not_found()
        {
            await using var provider = SetupServiceCollection();

            var harness = provider.GetTestHarness();

            await harness.Start();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(10);

            var jobId = NewId.NextGuid();

            IRequestClient<GetJobState> stateClient = harness.GetRequestClient<GetJobState>();

            var jobState = await stateClient.GetJobState(jobId);

            Assert.That(jobState.CurrentState, Is.EqualTo("NotFound"));

            await harness.Stop();
        }

        static ServiceProvider SetupServiceCollection()
        {
            var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.SetKebabCaseEndpointNameFormatter();

                    x.AddConsumer<OddJobConsumer>()
                        .Endpoint(e => e.Name = "odd-job");

                    x.AddConsumer<OddJobCompletedConsumer>()
                        .Endpoint(e => e.ConcurrentMessageLimit = 1);

                    x.AddJobSagaStateMachines(options => options.SlotWaitTime = TimeSpan.FromSeconds(10));

                    x.UsingInMemory((context, cfg) =>
                    {
                        cfg.UseDelayedMessageScheduler();

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            harness.TestInactivityTimeout = TimeSpan.FromSeconds(10);

            return provider;
        }
    }
}
