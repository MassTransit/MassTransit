namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading.Tasks;
    using Contracts.JobService;
    using global::Azure;
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
    public class JobConsumer_Specs
    {
        [Test]
        public async Task Should_cancel_the_job()
        {
            await using var provider = SetupServiceCollection();

            var harness = await provider.StartTestHarness();

            var jobId = NewId.NextGuid();

            IRequestClient<SubmitJob<OddJob>> client = harness.GetRequestClient<SubmitJob<OddJob>>();

            MassTransit.Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = jobId,
                Job = new { Duration = TimeSpan.FromSeconds(10) }
            });

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(response.Message.JobId, Is.EqualTo(jobId));

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

            var harness = await provider.StartTestHarness();

            var jobId = NewId.NextGuid();

            IRequestClient<SubmitJob<OddJob>> client = harness.GetRequestClient<SubmitJob<OddJob>>();

            MassTransit.Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = jobId,
                Job = new { Duration = TimeSpan.FromSeconds(10) }
            });

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(response.Message.JobId, Is.EqualTo(jobId));

                Assert.That(await harness.Published.Any<JobSubmitted>(), Is.True);

                Assert.That(await harness.Published.Any<JobStarted>(), Is.True);
                Assert.That(await harness.Published.Any<JobStarted<OddJob>>(), Is.True);
            });

            IRequestClient<GetJobState> stateClient = harness.GetRequestClient<GetJobState>();

            MassTransit.Response<JobState> jobState = await stateClient.GetResponse<JobState>(new { JobId = jobId });

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

            var harness = await provider.StartTestHarness();

            var jobId = NewId.NextGuid();

            IRequestClient<SubmitJob<OddJob>> client = harness.GetRequestClient<SubmitJob<OddJob>>();

            MassTransit.Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = jobId,
                Job = new { Duration = TimeSpan.FromSeconds(10) }
            });

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(response.Message.JobId, Is.EqualTo(jobId));

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

            var harness = await provider.StartTestHarness();

            var previousJobId = NewId.NextGuid();

            var jobId = NewId.NextGuid();

            IRequestClient<SubmitJob<OddJob>> client = harness.GetRequestClient<SubmitJob<OddJob>>();

            MassTransit.Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = previousJobId,
                Job = new { Duration = TimeSpan.FromSeconds(10) }
            });

            Assert.That(await harness.Published.Any<JobStarted>(x => x.Context.Message.JobId == previousJobId), Is.True);

            response = await client.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = jobId,
                Job = new { Duration = TimeSpan.FromSeconds(10) }
            });

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await harness.Published.Any<JobSubmitted>(x => x.Context.Message.JobId == jobId), Is.True);

                Assert.That(response.Message.JobId, Is.EqualTo(jobId));

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

            var harness = await provider.StartTestHarness();

            var jobId = NewId.NextGuid();

            IRequestClient<SubmitJob<OddJob>> client = harness.GetRequestClient<SubmitJob<OddJob>>();

            MassTransit.Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new
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

            var harness = await provider.StartTestHarness();

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

            var harness = await provider.StartTestHarness();

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
                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10), testTimeout: TimeSpan.FromMinutes(1));
                    x.SetKebabCaseEndpointNameFormatter();

                    x.AddConsumer<OddJobConsumer>()
                        .Endpoint(e => e.Name = "odd-job");

                    x.AddConsumer<OddJobCompletedConsumer>()
                        .Endpoint(e => e.ConcurrentMessageLimit = 1);

                    x.AddJobSagaStateMachines();
                    x.SetJobConsumerOptions(options => options.HeartbeatInterval = TimeSpan.FromSeconds(10))
                        .Endpoint(e => e.PrefetchCount = 100);

                    x.UsingTestAzureServiceBus();
                })
                .BuildServiceProvider(true);

            return provider;
        }
    }
}
