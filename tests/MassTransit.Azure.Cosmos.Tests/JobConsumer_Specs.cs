namespace MassTransit.Azure.Cosmos.Tests
{
    using System;
    using System.Threading.Tasks;
    using Contracts.JobService;
    using JobConsumerTests;
    using Logging;
    using Microsoft.Azure.Cosmos;
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


    public class Using_the_new_job_service_configuration
    {
        readonly string _collectionName;
        readonly string _databaseName;
        Container _container;
        CosmosClient _cosmosClient;

        Database _database;

        public Using_the_new_job_service_configuration()
        {
            _databaseName = "jobService";
            _collectionName = "sagas";
        }

        [Test]
        public async Task Should_complete_the_job()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddOptions<TextWriterLoggerOptions>().Configure(options => options.Disable("Microsoft"));

                    x.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));

                    x.SetKebabCaseEndpointNameFormatter();

                    x.AddConsumer<OddJobConsumer>()
                        .Endpoint(e => e.Name = "odd-job");

                    x.AddConsumer<OddJobCompletedConsumer>()
                        .Endpoint(e => e.ConcurrentMessageLimit = 1);

                    x.SetJobConsumerOptions(options => options.HeartbeatInterval = TimeSpan.FromSeconds(10))
                        .Endpoint(e => e.PrefetchCount = 100);

                    x.AddJobSagaStateMachines()
                        .JobEndpoint(e => e.Name = "da-job")
                        .JobAttemptEndpoint(e => e.Name = "da_job-attempt")
                        .CosmosRepository(r =>
                        {
                            r.AccountEndpoint = Configuration.AccountEndpoint;
                            r.AuthKeyOrResourceToken = Configuration.AccountKey;

                            r.DatabaseId = _databaseName;
                            r.CollectionId = _collectionName;
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
            }
            finally
            {
                await harness.Stop();
            }
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _cosmosClient = new CosmosClient(Configuration.AccountEndpoint, Configuration.AccountKey);

            var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName).ConfigureAwait(false);
            _database = databaseResponse.Database;

            var containerResponse = await _database.CreateContainerIfNotExistsAsync(_collectionName, "/id").ConfigureAwait(false);
            _container = containerResponse.Container;
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _container.DeleteContainerAsync().ConfigureAwait(false);
            await _database.DeleteAsync().ConfigureAwait(false);
        }
    }
}
