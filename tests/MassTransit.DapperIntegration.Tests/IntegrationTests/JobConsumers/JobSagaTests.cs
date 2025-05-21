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
    using Npgsql;
    using NUnit.Framework;
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

    [Category("Integration")]
    [TestFixture(typeof(SqlServerConnector))]
    [TestFixture(typeof(PostgresConnector), Explicit = true)]
    public class JobSagaTests<TConnector> : InMemoryTestFixture
        where TConnector : TestConnector, new()
    {
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
                .DapperRepository(conf => _connector.Connect(conf));
            
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
        public Task Setup() => _connector.Setup();

        [TearDown]
        public Task Reset() => _connector.Reset();

        [OneTimeTearDown]
        public Task TearDown() => _connector.Teardown();

    }


    public class PostgresConnector : TestConnector
    {
        readonly string _connectionString;

        public PostgresConnector()
        {
            _connectionString = "Host=localhost; Username=postgres; Password=Password12!; Database=masstransit";
        }

        public async Task Setup()
        {
            await RunSql(Sql.Postgres_DropJobTables);
            await RunSql(Sql.Postgres_CreateJobTables);
        }

        public Task Reset() => RunSql(Sql.Postgres_ResetJobTables);

        public Task Teardown() => RunSql(Sql.Postgres_DropJobTables);

        public void Connect(IDapperJobSagaRepositoryConfigurator conf)
        {
            conf.UsePostgres(_connectionString);
        }

        async Task RunSql(string sql)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = sql;

            await command.ExecuteNonQueryAsync();
        }
    }


    public class SqlServerConnector : TestConnector
    {
        readonly string _connectionString;

        public SqlServerConnector()
        {
            _connectionString = LocalDbConnectionStringProvider.GetLocalDbConnectionString();
        }

        public async Task Setup()
        {
            await RunSql(Sql.SqlServer_DropJobTables);
            await RunSql(Sql.SqlServer_CreateJobTables);
        }

        public Task Reset()
        {
            return RunSql(Sql.SqlServer_ResetJobTables);
        }

        public Task Teardown()
        {
            return RunSql(Sql.SqlServer_DropJobTables);
        }

        public void Connect(IDapperJobSagaRepositoryConfigurator conf)
        {
            conf.UseSqlServer(_connectionString);
        }

        async Task RunSql(string sql)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = sql;

            await command.ExecuteNonQueryAsync();
        }
    }
    
    public interface TestConnector
    {
        Task Setup();
        Task Reset();
        Task Teardown();
        void Connect(IDapperJobSagaRepositoryConfigurator conf);
    }
}
