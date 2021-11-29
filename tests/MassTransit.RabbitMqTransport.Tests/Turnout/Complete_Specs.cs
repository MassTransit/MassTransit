namespace MassTransit.RabbitMqTransport.Tests.Turnout
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Contracts.JobService;
    using NUnit.Framework;


    public interface CrunchTheNumbers
    {
        TimeSpan Duration { get; }
    }


    public interface NumbersCrunched
    {
        Guid JobId { get; }
    }


    public class CrunchTheNumbersConsumer :
        IJobConsumer<CrunchTheNumbers>
    {
        public async Task Run(JobContext<CrunchTheNumbers> context)
        {
            await Task.Delay(context.Job.Duration);

            await context.Publish<NumbersCrunched>(new { context.JobId });
        }
    }


    [TestFixture]
    [Category("Flaky")]
    public class Submitting_a_job_to_turnout :
        RabbitMqTestFixture
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

        [Test]
        [Order(5)]
        public async Task Should_have_published_the_numbers_crunched_event()
        {
            ConsumeContext<NumbersCrunched> completed = await _crunched;
        }

        Guid _jobId;
        Task<ConsumeContext<JobCompleted>> _completed;
        Task<ConsumeContext<JobSubmitted>> _submitted;
        Task<ConsumeContext<JobStarted>> _started;
        Task<ConsumeContext<NumbersCrunched>> _crunched;

        [OneTimeSetUp]
        public async Task Arrange()
        {
            _jobId = NewId.NextGuid();
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();

            var options = new ServiceInstanceOptions()
                .SetEndpointNameFormatter(KebabCaseEndpointNameFormatter.Instance);

            configurator.ServiceInstance(options, instance =>
            {
                instance.ConfigureJobServiceEndpoints();

                instance.ReceiveEndpoint(instance.EndpointNameFormatter.Message<CrunchTheNumbers>(), e =>
                {
                    e.UseInMemoryOutbox();
                    e.Consumer(() => new CrunchTheNumbersConsumer());
                });
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _submitted = Handled<JobSubmitted>(configurator, context => context.Message.JobId == _jobId);
            _started = Handled<JobStarted>(configurator, context => context.Message.JobId == _jobId);
            _completed = Handled<JobCompleted>(configurator, context => context.Message.JobId == _jobId);
            _crunched = Handled<NumbersCrunched>(configurator, context => context.Message.JobId == _jobId);
        }
    }


    [TestFixture]
    [Explicit]
    public class Submitting_a_job_to_turnout_with_status_checks :
        RabbitMqTestFixture
    {
        [Test]
        [Order(1)]
        public async Task Should_get_the_job_accepted()
        {
            IRequestClient<SubmitJob<CrunchTheNumbers>> requestClient = Bus.CreateRequestClient<SubmitJob<CrunchTheNumbers>>();

            Response<JobSubmissionAccepted> response = await requestClient.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = _jobId,
                Job = new { Duration = TimeSpan.FromMinutes(3.5) }
            });

            Assert.That(response.Message.JobId, Is.EqualTo(_jobId));

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

        public Submitting_a_job_to_turnout_with_status_checks()
        {
            TestTimeout = TimeSpan.FromMinutes(5);
            TestInactivityTimeout = TimeSpan.FromMinutes(1);
        }

        Guid _jobId;
        Task<ConsumeContext<JobCompleted>> _completed;
        Task<ConsumeContext<JobSubmitted>> _submitted;
        Task<ConsumeContext<JobStarted>> _started;

        [OneTimeSetUp]
        public async Task Arrange()
        {
            _jobId = NewId.NextGuid();
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();

            var options = new ServiceInstanceOptions()
                .SetEndpointNameFormatter(KebabCaseEndpointNameFormatter.Instance);

            configurator.ServiceInstance(options, instance =>
            {
                instance.ConfigureJobServiceEndpoints();

                instance.ReceiveEndpoint(instance.EndpointNameFormatter.Message<CrunchTheNumbers>(), e =>
                {
                    e.Consumer(() => new CrunchTheNumbersConsumer());
                });
            });
        }

        protected override void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            _submitted = Handled<JobSubmitted>(configurator, context => context.Message.JobId == _jobId);
            _started = Handled<JobStarted>(configurator, context => context.Message.JobId == _jobId);
            _completed = Handled<JobCompleted>(configurator, context => context.Message.JobId == _jobId);
        }
    }
}
