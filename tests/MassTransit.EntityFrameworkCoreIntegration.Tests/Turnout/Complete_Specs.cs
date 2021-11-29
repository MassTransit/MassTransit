namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Turnout
{
    using System;
    using System.Threading.Tasks;
    using Contracts.JobService;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using Shared;


    public interface CrunchTheNumbers
    {
        TimeSpan Duration { get; }
    }


    public interface NumbersCrunched
    {
        Guid JobId { get; }
        TimeSpan ElapsedTime { get; }
    }


    public class CrunchTheNumbersConsumer :
        IJobConsumer<CrunchTheNumbers>
    {
        public async Task Run(JobContext<CrunchTheNumbers> context)
        {
            await Task.Delay(context.Job.Duration);
        }
    }


    public class CrunchTheNumbersContainerConsumer :
        IJobConsumer<CrunchTheNumbers>
    {
        readonly IPublishEndpoint _publishEndpoint;

        public CrunchTheNumbersContainerConsumer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Run(JobContext<CrunchTheNumbers> context)
        {
            await Task.Delay(context.Job.Duration);

            await _publishEndpoint.Publish<NumbersCrunched>(new
            {
                context.JobId,
                context.ElapsedTime
            });
        }
    }


    [Category("Flaky")]
    [TestFixture(typeof(SqlServerTestDbParameters))]
    [TestFixture(typeof(PostgresTestDbParameters))]
    public class Submitting_a_job_to_turnout<T> :
        QuartzEntityFrameworkTestFixture<T, JobServiceSagaDbContext>
        where T : ITestDbParameters, new()
    {
        [Test]
        [Order(1)]
        public async Task Should_get_the_job_accepted()
        {
            IRequestClient<SubmitJob<CrunchTheNumbers>> requestClient = Bus.CreateRequestClient<SubmitJob<CrunchTheNumbers>>();

            Response<JobSubmissionAccepted> response = await requestClient.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = _jobId,
                Job = new { Duration = TimeSpan.FromSeconds(1) }
            });

            Assert.That(response.Message.JobId, Is.EqualTo(_jobId));

            // just to capture all the test output in a single window
            ConsumeContext<JobCompleted> completed = await _completed;
        }

        [Test]
        [Order(4)]
        public async Task Should_have_published_the_job_completed_event()
        {
            ConsumeContext<JobCompleted> completed = await _completed;
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
        Task<ConsumeContext<JobCompleted>> _completed;
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

                instance.ReceiveEndpoint(instance.EndpointNameFormatter.Message<CrunchTheNumbers>(), e =>
                {
                    e.Consumer(() => new CrunchTheNumbersConsumer());
                });
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _submitted = Handled<JobSubmitted>(configurator, context => context.Message.JobId == _jobId);
            _started = Handled<JobStarted>(configurator, context => context.Message.JobId == _jobId);
            _completed = Handled<JobCompleted>(configurator, context => context.Message.JobId == _jobId);
        }
    }


    [TestFixture(typeof(SqlServerTestDbParameters))]
    [TestFixture(typeof(PostgresTestDbParameters))]
    [Category("Flaky")]
    public class Submitting_a_job_to_turnout_via_container<T> :
        QuartzEntityFrameworkTestFixture<T, JobServiceSagaDbContext>
        where T : ITestDbParameters, new()
    {
        [Test]
        [Order(1)]
        public async Task Should_get_the_job_accepted()
        {
            var requestClient = _provider.GetRequiredService<IRequestClient<SubmitJob<CrunchTheNumbers>>>();

            Response<JobSubmissionAccepted> response = await requestClient.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = _jobId,
                Job = new { Duration = TimeSpan.FromSeconds(1) }
            });

            Assert.That(response.Message.JobId, Is.EqualTo(_jobId));

            // just to capture all the test output in a single window
            ConsumeContext<JobCompleted> completed = await _completed;
        }

        [Test]
        [Order(4)]
        public async Task Should_have_published_the_job_completed_event()
        {
            ConsumeContext<JobCompleted> completed = await _completed;
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
        Task<ConsumeContext<JobCompleted>> _completed;
        Task<ConsumeContext<JobSubmitted>> _submitted;
        Task<ConsumeContext<JobStarted>> _started;
        readonly IServiceProvider _provider;

        public Submitting_a_job_to_turnout_via_container()
        {
            _provider = new ServiceCollection()
                .AddDbContext<JobServiceSagaDbContext>(builder => ApplyBuilderOptions(builder))
                .AddMassTransit(x =>
                {
                    x.AddConsumer<CrunchTheNumbersContainerConsumer>();

                    x.AddRequestClient<SubmitJob<CrunchTheNumbers>>();

                    x.AddSagaRepository<JobSaga>()
                        .EntityFrameworkRepository(r =>
                        {
                            r.ExistingDbContext<JobServiceSagaDbContext>();
                            r.LockStatementProvider = RawSqlLockStatements;
                        });
                    x.AddSagaRepository<JobTypeSaga>()
                        .EntityFrameworkRepository(r =>
                        {
                            r.ExistingDbContext<JobServiceSagaDbContext>();
                            r.LockStatementProvider = RawSqlLockStatements;
                        });
                    x.AddSagaRepository<JobAttemptSaga>()
                        .EntityFrameworkRepository(r =>
                        {
                            r.ExistingDbContext<JobServiceSagaDbContext>();
                            r.LockStatementProvider = RawSqlLockStatements;
                        });

                    x.AddBus(provider => BusControl);
                })
                .BuildServiceProvider();
        }

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
                var busRegistrationContext = _provider.GetRequiredService<IBusRegistrationContext>();

                instance.ConfigureJobServiceEndpoints(x =>
                {
                    x.ConfigureSagaRepositories(busRegistrationContext);
                });

                instance.ReceiveEndpoint(instance.EndpointNameFormatter.Message<CrunchTheNumbers>(), e =>
                {
                    e.ConfigureConsumer<CrunchTheNumbersContainerConsumer>(busRegistrationContext);
                });
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _submitted = Handled<JobSubmitted>(configurator, context => context.Message.JobId == _jobId);
            _started = Handled<JobStarted>(configurator, context => context.Message.JobId == _jobId);
            _completed = Handled<JobCompleted>(configurator, context => context.Message.JobId == _jobId);
        }
    }
}
