namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Internals;
    using JobConsumerTests;
    using Logging;
    using MassTransit.Contracts.JobService;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using NUnit.Framework;
    using Testing;
    using Util;


    namespace JobConsumerTests
    {
        using System;
        using System.Threading.Tasks;
        using MassTransit.Contracts.JobService;


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
        [Explicit]
        public async Task Should_cancel_on_shutdown_and_then_restart_the_job()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransit(x =>
                {
                    x.AddOptions<TextWriterLoggerOptions>();
                    x.TryAddSingleton<ILoggerFactory>(provider =>
                        new TextWriterLoggerFactory(Console.Out, provider.GetRequiredService<IOptions<TextWriterLoggerOptions>>()));
                    x.TryAddSingleton(typeof(ILogger<>), typeof(Logger<>));

                    x.AddOptions<RabbitMqTransportOptions>()
                        .Configure(options => options.VHost = "test");

                    x.SetKebabCaseEndpointNameFormatter();

                    x.AddConsumer<OddJobConsumer>()
                        .Endpoint(e => e.Name = "odd-job");

                    x.AddConsumer<OddJobCompletedConsumer>()
                        .Endpoint(e => e.ConcurrentMessageLimit = 1);

                    x.SetInMemorySagaRepositoryProvider();

                    x.AddJobSagaStateMachines(options =>
                    {
                        options.SlotWaitTime = TimeSpan.FromSeconds(1);
                    });
                    x.SetJobConsumerOptions(options => options.HeartbeatInterval = TimeSpan.FromSeconds(10))
                        .Endpoint(e => e.PrefetchCount = 100);

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.UseDelayedMessageScheduler();

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

            IHostedService[] services = provider.GetServices<IHostedService>().ToArray();

            foreach (var service in services)
                await service.StartAsync(CancellationToken.None).ConfigureAwait(false);

            var jobId = NewId.NextGuid();

            var connector = provider.GetRequiredService<IReceiveEndpointConnector>();

            TaskCompletionSource<bool> started = TaskUtil.GetTask<bool>();
            TaskCompletionSource<bool> completed = TaskUtil.GetTask<bool>();

            var handle = connector.ConnectReceiveEndpoint("observers", (context, cfg) =>
            {
                cfg.Handler<JobStarted>(async e => started.TrySetResult(true));
                cfg.Handler<JobCompleted>(async e => completed.TrySetResult(true));
            });

            await handle.Ready;

            await using var scope = provider.CreateAsyncScope();

            _ = await scope.ServiceProvider.GetRequiredService<IRequestClient<SubmitJob<OddJob>>>()
                .SubmitJob(jobId, new { Duration = TimeSpan.FromSeconds(5) });

            await started.Task.OrTimeout(TimeSpan.FromSeconds(10));

            await Task.Delay(1000);

            foreach (var service in services.Reverse())
                await service.StopAsync(CancellationToken.None).ConfigureAwait(false);

            await Task.Delay(1000);

            foreach (var service in services)
                await service.StartAsync(CancellationToken.None).ConfigureAwait(false);

            await completed.Task.OrTimeout(TimeSpan.FromSeconds(10));
        }

        [Test]
        public async Task Should_cancel_the_job()
        {
            await using var provider = SetupServiceCollection();

            var harness = provider.GetTestHarness();

            await harness.Start();

            var jobId = NewId.NextGuid();

            IRequestClient<SubmitJob<OddJob>> client = harness.GetRequestClient<SubmitJob<OddJob>>();

            Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new
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

            var harness = provider.GetTestHarness();

            await harness.Start();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(10);

            var jobId = NewId.NextGuid();

            IRequestClient<SubmitJob<OddJob>> client = harness.GetRequestClient<SubmitJob<OddJob>>();

            Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new
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

            Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new
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

            var harness = provider.GetTestHarness();
            harness.TestInactivityTimeout = TimeSpan.FromSeconds(10);

            await harness.Start();

            var previousJobId = NewId.NextGuid();

            var jobId = NewId.NextGuid();

            IRequestClient<SubmitJob<OddJob>> client = harness.GetRequestClient<SubmitJob<OddJob>>();

            await client.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = previousJobId,
                Job = new { Duration = TimeSpan.FromSeconds(10) }
            });

            Assert.That(await harness.Published.Any<JobStarted>(x => x.Context.Message.JobId == previousJobId), Is.True);

            Response<JobSubmissionAccepted> response = await client.GetResponse<JobSubmissionAccepted>(new
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

        static ServiceProvider SetupServiceCollection(bool cleanVirtualHost = true)
        {
            var provider = new ServiceCollection()
                .ConfigureRabbitMqTestOptions(options =>
                {
                    options.CleanVirtualHost = cleanVirtualHost;
                    options.CreateVirtualHostIfNotExists = true;
                })
                .AddMassTransitTestHarness(x =>
                {
                    x.AddOptions<RabbitMqTransportOptions>()
                        .Configure(options => options.VHost = "test");

                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));
                    x.SetKebabCaseEndpointNameFormatter();

                    x.AddConsumer<OddJobConsumer>()
                        .Endpoint(e => e.Name = "odd-job");

                    x.AddConsumer<OddJobCompletedConsumer>()
                        .Endpoint(e => e.ConcurrentMessageLimit = 1);

                    x.AddJobSagaStateMachines();
                    x.SetJobConsumerOptions(options =>
                        {
                            options.HeartbeatInterval = TimeSpan.FromSeconds(10);
                        })
                        .Endpoint(e =>
                        {
                            e.PrefetchCount = 100;

                            e.AddConfigureEndpointCallback(cfg =>
                            {
                                if (cfg is IRabbitMqReceiveEndpointConfigurator rmq)
                                    rmq.SetQuorumQueue();
                            });
                        });

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
                })
                .BuildServiceProvider(true);

            var harness = provider.GetTestHarness();

            harness.TestInactivityTimeout = TimeSpan.FromSeconds(10);

            return provider;
        }
    }
}
