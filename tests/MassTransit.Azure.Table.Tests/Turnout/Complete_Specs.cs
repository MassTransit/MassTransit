namespace MassTransit.Azure.Table.Tests.Turnout
{
    using System;
    using System.Threading.Tasks;
    using Contracts.JobService;
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


    [TestFixture]
    [Category("Flaky")]
    public class Submitting_a_job_to_turnout :
        AzureTableInMemoryTestFixture
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
        [Order(4)]
        public async Task Should_have_published_the_job_completed_generic_event()
        {
            ConsumeContext<JobCompleted<CrunchTheNumbers>> completed = await _completedT;
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
        Task<ConsumeContext<JobCompleted>> _completed;
        Task<ConsumeContext<JobSubmitted>> _submitted;
        Task<ConsumeContext<JobStarted>> _started;
        Task<ConsumeContext<JobCompleted<CrunchTheNumbers>>> _completedT;

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            var options = new ServiceInstanceOptions()
                .SetEndpointNameFormatter(KebabCaseEndpointNameFormatter.Instance);

            configurator.ServiceInstance(options, instance =>
            {
                instance.ConfigureJobServiceEndpoints(x =>
                {
                    x.UseAzureTableSagaRepository(() => TestCloudTable);
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
            _completedT = Handled<JobCompleted<CrunchTheNumbers>>(configurator, context => context.Message.JobId == _jobId);
        }
    }
}
