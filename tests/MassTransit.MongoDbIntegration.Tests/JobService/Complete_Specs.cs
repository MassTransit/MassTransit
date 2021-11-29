namespace MassTransit.MongoDbIntegration.Tests.JobService
{
    using System;
    using System.Threading.Tasks;
    using Contracts.JobService;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    public interface CrunchTheNumbers
    {
        TimeSpan Duration { get; }
    }


    public class CrunchTheNumbersConsumer :
        IJobConsumer<CrunchTheNumbers>
    {
        public async Task Run(JobContext<CrunchTheNumbers> context)
        {
            await Task.Delay(context.Job.Duration);
        }
    }


    public class Submitting_a_job_to_turnout_via_container :
        QuartzInMemoryTestFixture
    {
        readonly IServiceProvider _provider;
        Task<ConsumeContext<JobCompleted>> _completed;

        Guid _jobId;
        Task<ConsumeContext<JobStarted>> _started;
        Task<ConsumeContext<JobSubmitted>> _submitted;

        public Submitting_a_job_to_turnout_via_container()
        {
            _provider = new ServiceCollection()
                .AddMassTransit(x =>
                {
                    x.AddConsumer<CrunchTheNumbersConsumer>();

                    x.AddRequestClient<SubmitJob<CrunchTheNumbers>>();

                    x.AddSagaRepository<JobSaga>()
                        .MongoDbRepository(r =>
                        {
                            r.Connection = "mongodb://127.0.0.1";
                            r.DatabaseName = "sagaTest";
                        });
                    x.AddSagaRepository<JobTypeSaga>()
                        .MongoDbRepository(r =>
                        {
                            r.Connection = "mongodb://127.0.0.1";
                            r.DatabaseName = "sagaTest";
                        });
                    x.AddSagaRepository<JobAttemptSaga>()
                        .MongoDbRepository(r =>
                        {
                            r.Connection = "mongodb://127.0.0.1";
                            r.DatabaseName = "sagaTest";
                        });

                    x.AddBus(provider => BusControl);
                })
                .BuildServiceProvider();
        }

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

        [OneTimeSetUp]
        public async Task Arrange()
        {
            _jobId = NewId.NextGuid();
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
                    e.ConfigureConsumer<CrunchTheNumbersConsumer>(busRegistrationContext);
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
