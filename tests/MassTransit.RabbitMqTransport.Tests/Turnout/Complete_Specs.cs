namespace MassTransit.RabbitMqTransport.Tests.Turnout
{
    using System;
    using System.Threading.Tasks;
    using Conductor.Configuration;
    using Definition;
    using MassTransit.Contracts.Turnout;
    using NUnit.Framework;


    public interface CrunchTheNumbers
    {
        TimeSpan Duration { get; }
    }


    [TestFixture]
    public class Submitting_a_job_to_turnout :
        RabbitMqTestFixture
    {
        Guid _jobId;
        Task<ConsumeContext<JobCompleted>> _completed;
        Task<ConsumeContext<JobSubmitted>> _submitted;
        Task<ConsumeContext<JobStarted>> _started;

        [Test]
        [Order(1)]
        public async Task Should_get_the_job_accepted()
        {
            var serviceClient = Bus.CreateServiceClient();

            var requestClient = serviceClient.CreateRequestClient<SubmitJob<CrunchTheNumbers>>();

            var response = await requestClient.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = _jobId,
                Job = new {Duration = TimeSpan.FromSeconds(1)}
            });

            Assert.That(response.Message.JobId, Is.EqualTo(_jobId));

            // just to capture all the test output in a single window
            var completed = await _completed;
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
        public async Task Should_have_published_the_job_completed_event()
        {
            var completed = await _completed;
        }

        [OneTimeSetUp]
        public async Task Arrange()
        {
            _jobId = NewId.NextGuid();
        }

        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedExchangeMessageScheduler();

            var options = new ServiceInstanceOptions()
                .EnableInstanceEndpoint()
                .SetEndpointNameFormatter(KebabCaseEndpointNameFormatter.Instance);

            configurator.ServiceInstance(options, instance =>
            {
                instance.Turnout(x =>
                {
                    x.Job<CrunchTheNumbers>(cfg =>
                    {
                        cfg.ConcurrentJobLimit = 10;

                        cfg.JobTimeout = TimeSpan.FromSeconds(90);

                        cfg.SetJobFactory(async context =>
                        {
                            await Task.Delay(context.Job.Duration);
                        });
                    });
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
