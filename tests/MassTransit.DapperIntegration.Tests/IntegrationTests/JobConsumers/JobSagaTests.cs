namespace MassTransit.DapperIntegration.Tests.IntegrationTests
{
    using System;
    using System.Threading.Tasks;
    using Configuration;
    using Contracts.JobService;
    using MassTransit.Tests;
    using MassTransit.Tests.JobConsumerTests;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Saga;
    using TestFramework;
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

    [TestFixture]
    public class JobSagaTests : InMemoryTestFixture
    {
        readonly string _connectionString;

        public JobSagaTests()
        {
            _connectionString = LocalDbConnectionStringProvider.GetLocalDbConnectionString();
        }

        void ConfigureRegistration(IBusRegistrationConfigurator configurator)
        {
            configurator.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(10));
            configurator.SetKebabCaseEndpointNameFormatter();

            configurator.AddConsumer<OddJobConsumer>().Endpoint(e => e.Name = "odd-job");
            configurator.AddConsumer<OddJobCompletedConsumer>().Endpoint(e => e.ConcurrentMessageLimit = 1);
            configurator.SetJobConsumerOptions(options => options.HeartbeatInterval = TimeSpan.FromSeconds(10)).Endpoint(e => e.PrefetchCount = 100);

            configurator.AddJobSagaStateMachines()
                .DapperRepository(conf => conf.UseSqlServer(_connectionString));
            
            configurator.UsingInMemory((ctx, cfg) =>
            {
                cfg.UseDelayedMessageScheduler();
                cfg.ConfigureEndpoints(ctx);
            });
        }

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

        [OneTimeSetUp]
        public async Task Arrange()
        {
            await RunSql(Sql.DropJobTables);
            await RunSql(Sql.CreateJobTables);
        }

        [TearDown]
        public async Task Reset()
        {
            await RunSql(Sql.ResetJobTables);
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await RunSql(Sql.DropJobTables);
        }

        protected async Task RunSql(string sql)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = sql;

            await command.ExecuteNonQueryAsync();
        }
    }
}
