namespace MassTransit.Persistence.Tests.IntegrationTests.JobConsumers
{
    using Configuration;
    using Connectors;
    using Contracts.JobService;
    using MassTransit.Tests.JobConsumerTests;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    namespace JobConsumerTests
    {
        using Contracts.JobService;


        public interface OddJob
        {
            TimeSpan Duration { get; }
        }


        public class OddJobConsumer : IJobConsumer<OddJob>
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


    [Category("Integration")]
    [TestFixture(typeof(PessimisticSqlServerConnector))]
    [TestFixture(typeof(PessimisticPostgresConnector), Explicit = true)]
    [TestFixture(typeof(PessimisticMySqlConnector), Explicit = true)]
    public class JobSagaTests<TConnector> : InMemoryTestFixture
        where TConnector : TestConnector, new()
    {
        [Test]
        public async Task Starting_job_will_start_job()
        {
            var services = new ServiceCollection();

            await using var provider = services
                .AddMassTransitTestHarness(ConfigureRegistration)
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

        [SetUp]
        public Task Setup()
        {
            return _connector.Setup();
        }

        [TearDown]
        public Task TearDown()
        {
            return _connector.Teardown();
        }

        readonly TestConnector _connector;

        public JobSagaTests()
        {
            _connector = new TConnector();
        }

        void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));
            configurator.SetKebabCaseEndpointNameFormatter();

            configurator.AddConsumer<OddJobConsumer>().Endpoint(e => e.Name = "odd-job");
            configurator.AddConsumer<OddJobCompletedConsumer>().Endpoint(e => e.ConcurrentMessageLimit = 1);
            configurator.SetJobConsumerOptions(options => options.HeartbeatInterval = TimeSpan.FromSeconds(10)).Endpoint(e => e.PrefetchCount = 100);

            configurator.AddJobSagaStateMachines()
                .CustomRepository(_connector.Connect);

            configurator.UsingInMemory((ctx, cfg) =>
            {
                cfg.UseDelayedMessageScheduler();
                cfg.ConfigureEndpoints(ctx);
            });
        }
    }
}
