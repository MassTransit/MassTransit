namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Turnout
{
    using System;
    using System.Threading.Tasks;
    using Contracts.JobService;
    using NUnit.Framework;
    using Shared;


    [Category("Flaky")]
    [TestFixture(typeof(SqlServerTestDbParameters))]
    [TestFixture(typeof(PostgresTestDbParameters))]
    public class Submitting_a_job_to_turnout_that_is_cancelled<T> :
        QuartzEntityFrameworkTestFixture<T, JobServiceSagaDbContext>
        where T : ITestDbParameters, new()
    {
        [Test]
        [Order(1)]
        public async Task Should_get_the_job_accepted()
        {
            IRequestClient<SubmitJob<GrindTheGears>> requestClient = Bus.CreateRequestClient<SubmitJob<GrindTheGears>>();

            Response<JobSubmissionAccepted> response = await requestClient.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = _jobId,
                Job = new { Duration = TimeSpan.FromSeconds(30) }
            });

            Assert.That(response.Message.JobId, Is.EqualTo(_jobId));

            ConsumeContext<JobStarted> started = await _started;

            await Bus.Publish<CancelJob>(new
            {
                JobId = _jobId,
                Reason = "I give up",
                InVar.Timestamp
            });

            // just to capture all the test output in a single window
            ConsumeContext<JobCanceled> cancelled = await _cancelled;
        }

        [Test]
        [Order(4)]
        public async Task Should_have_published_the_job_cancelled_event()
        {
            ConsumeContext<JobCanceled> cancelled = await _cancelled;
        }

        [Test]
        [Order(3)]
        public async Task Should_have_published_the_job_started_event()
        {
            ConsumeContext<JobStarted> started = await _started;
        }

        [Test]
        [Order(2)]
        public async Task Should_have_published_the_job_submitted_event()
        {
            ConsumeContext<JobSubmitted> submitted = await _submitted;
        }

        Guid _jobId;
        Task<ConsumeContext<JobCanceled>> _cancelled;
        Task<ConsumeContext<JobSubmitted>> _submitted;
        Task<ConsumeContext<JobStarted>> _started;

        [OneTimeSetUp]
        public async Task Arrange()
        {
            _jobId = NewId.NextGuid();

            await using var context = new JobServiceSagaDbContextFactory().CreateDbContext(DbContextOptionsBuilder);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await using var context = new JobServiceSagaDbContextFactory().CreateDbContext(DbContextOptionsBuilder);

            await context.Database.EnsureDeletedAsync();
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            var options = new ServiceInstanceOptions()
                .SetEndpointNameFormatter(KebabCaseEndpointNameFormatter.Instance);

            configurator.ServiceInstance(options, instance =>
            {
                instance.ConfigureJobServiceEndpoints(x =>
                {
                    x.UseEntityFrameworkCoreSagaRepository(() => new JobServiceSagaDbContextFactory().CreateDbContext(DbContextOptionsBuilder),
                        RawSqlLockStatements);
                });

                instance.ReceiveEndpoint(instance.EndpointNameFormatter.Message<GrindTheGears>(), e =>
                {
                    e.Consumer(() => new GrindTheGearsConsumer(), cfg =>
                    {
                        cfg.Options<JobOptions<GrindTheGears>>(jobOptions => jobOptions.SetJobTimeout(TimeSpan.FromSeconds(90)));
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


        public class GrindTheGearsConsumer :
            IJobConsumer<GrindTheGears>
        {
            public async Task Run(JobContext<GrindTheGears> context)
            {
                await Task.Delay(context.Job.Duration, context.CancellationToken);
            }
        }
    }
}
