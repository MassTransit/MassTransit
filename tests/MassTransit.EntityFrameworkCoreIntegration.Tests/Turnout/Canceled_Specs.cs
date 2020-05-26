namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Turnout
{
    using System;
    using System.Threading.Tasks;
    using Conductor.Configuration;
    using Contracts.Turnout;
    using Definition;
    using EntityFrameworkCoreIntegration.Turnout;
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using Shared;


    [TestFixture(typeof(SqlServerTestDbParameters))]
    [TestFixture(typeof(PostgresTestDbParameters))]
    public class Submitting_a_job_to_turnout_that_is_cancelled<T> :
        QuartzEntityFrameworkTestFixture<T, TurnoutSagaDbContext>
        where T : ITestDbParameters, new()
    {
        Guid _jobId;
        Task<ConsumeContext<JobCanceled>> _cancelled;
        Task<ConsumeContext<JobSubmitted>> _submitted;
        Task<ConsumeContext<JobStarted>> _started;

        [Test]
        [Order(1)]
        public async Task Should_get_the_job_accepted()
        {
            var serviceClient = Bus.CreateServiceClient();

            var requestClient = serviceClient.CreateRequestClient<SubmitJob<GrindTheGears>>();

            var response = await requestClient.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = _jobId,
                Job = new {Duration = TimeSpan.FromSeconds(30)}
            });

            Assert.That(response.Message.JobId, Is.EqualTo(_jobId));

            var started = await _started;

            await Bus.Publish<CancelJob>(new
            {
                JobId = _jobId,
                Reason = "I give up",
                InVar.Timestamp
            });

            // just to capture all the test output in a single window
            var cancelled = await _cancelled;
        }

        [Test]
        [Order(2)]
        public async Task Should_have_published_the_job_submitted_event()
        {
            var submitted = await _submitted;
        }

        [Test]
        [Order(3)]
        public async Task Should_have_published_the_job_started_event()
        {
            var started = await _started;
        }

        [Test]
        [Order(4)]
        public async Task Should_have_published_the_job_cancelled_event()
        {
            var cancelled = await _cancelled;
        }

        [OneTimeSetUp]
        public async Task Arrange()
        {
            _jobId = NewId.NextGuid();

            await using var context = new TurnoutSagaDbContextFactory().CreateDbContext(DbContextOptionsBuilder);

            await context.Database.MigrateAsync();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await using var context = new TurnoutSagaDbContextFactory().CreateDbContext(DbContextOptionsBuilder);

            await context.Database.EnsureDeletedAsync();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            var options = new ServiceInstanceOptions()
                .EnableInstanceEndpoint()
                .SetEndpointNameFormatter(KebabCaseEndpointNameFormatter.Instance);

            configurator.ServiceInstance(options, instance =>
            {
                instance.Turnout(x =>
                {
                    x.UseEntityFrameworkCoreSagaRepository(() => new TurnoutSagaDbContextFactory().CreateDbContext(DbContextOptionsBuilder),
                        RawSqlLockStatements);

                    x.Job<GrindTheGears>(cfg =>
                    {
                        cfg.ConcurrentJobLimit = 10;

                        cfg.JobTimeout = TimeSpan.FromSeconds(61);

                        cfg.SetJobFactory(async context =>
                        {
                            await Task.Delay(context.Job.Duration, context.CancellationToken);
                        });
                    });
                });
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _submitted = Handled<JobSubmitted>(configurator, context => context.Message.JobId == _jobId);
            _started = Handled<JobStarted>(configurator, context => context.Message.JobId == _jobId);
            _cancelled = Handled<JobCanceled>(configurator, context => context.Message.JobId == _jobId);
        }
    }
}
