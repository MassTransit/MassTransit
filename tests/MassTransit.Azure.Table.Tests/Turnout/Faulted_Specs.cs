namespace MassTransit.Azure.Table.Tests.Turnout
{
    using System;
    using System.Threading.Tasks;
    using Conductor;
    using Contracts.JobService;
    using Definition;
    using JobService;
    using NUnit.Framework;
    using TestFramework;
    using Tests;


    public interface GrindTheGears
    {
        TimeSpan Duration { get; }
    }

    public class Submitting_a_job_to_turnout_that_faults :
        AzureTableInMemoryTestFixture
    {
        [Test]
        [Order(1)]
        public async Task Should_get_the_job_accepted()
        {
            var serviceClient = Bus.CreateServiceClient();

            IRequestClient<SubmitJob<GrindTheGears>> requestClient = serviceClient.CreateRequestClient<SubmitJob<GrindTheGears>>();

            Response<JobSubmissionAccepted> response = await requestClient.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = _jobId,
                Job = new {Duration = TimeSpan.FromSeconds(1)}
            });

            Assert.That(response.Message.JobId, Is.EqualTo(_jobId));

            // just to capture all the test output in a single window
            ConsumeContext<JobFaulted> faulted = await _faulted;
        }

        [Test]
        [Order(4)]
        public async Task Should_have_published_the_job_faulted_event()
        {
            ConsumeContext<JobFaulted> faulted = await _faulted;
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

        readonly Guid _jobId = NewId.NextGuid();
        Task<ConsumeContext<JobFaulted>> _faulted;
        Task<ConsumeContext<JobSubmitted>> _submitted;
        Task<ConsumeContext<JobStarted>> _started;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            var options = new ServiceInstanceOptions()
                .EnableInstanceEndpoint()
                .SetEndpointNameFormatter(KebabCaseEndpointNameFormatter.Instance);

            configurator.ServiceInstance(options, instance =>
            {
                instance.ConfigureJobServiceEndpoints(x =>
                {
                    x.UseAzureTableSagaRepository(() => TestCloudTable);
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
            _faulted = Handled<JobFaulted>(configurator, context => context.Message.JobId == _jobId);
        }


        public class GrindTheGearsConsumer :
            IJobConsumer<GrindTheGears>
        {
            public async Task Run(JobContext<GrindTheGears> context)
            {
                await Task.Delay(context.Job.Duration);

                throw new IntentionalTestException("Grinding gears, dropped the transmission");
            }
        }
    }
}
