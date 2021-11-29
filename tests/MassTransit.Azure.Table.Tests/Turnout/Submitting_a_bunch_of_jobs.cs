namespace MassTransit.Azure.Table.Tests.Turnout
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts.JobService;
    using NUnit.Framework;


    [TestFixture]
    [Category("Flaky")]
    public class Submitting_a_bunch_of_jobs :
        AzureTableInMemoryTestFixture
    {
        [Test]
        [Order(1)]
        public async Task Should_get_the_job_accepted()
        {
            IRequestClient<SubmitJob<CrunchTheNumbers>> requestClient = Bus.CreateRequestClient<SubmitJob<CrunchTheNumbers>>();

            for (var i = 0; i < Count; i++)
            {
                Response<JobSubmissionAccepted> response = await requestClient.GetResponse<JobSubmissionAccepted>(new
                {
                    JobId = _jobIds[i],
                    Job = new { Duration = TimeSpan.FromSeconds(1) }
                });
            }

            ConsumeContext<JobCompleted>[] completed = await Task.WhenAll(_completed.Select(x => x.Task));
        }

        [Test]
        [Order(4)]
        public async Task Should_have_published_the_job_completed_event()
        {
            ConsumeContext<JobCompleted>[] completed = await Task.WhenAll(_completed.Select(x => x.Task));
        }

        [Test]
        [Order(3)]
        public async Task Should_have_published_the_job_started_event()
        {
            ConsumeContext<JobStarted>[] started = await Task.WhenAll(_started.Select(x => x.Task));
        }

        [Test]
        [Order(2)]
        public async Task Should_have_published_the_job_submitted_event()
        {
            ConsumeContext<JobSubmitted>[] submitted = await Task.WhenAll(_submitted.Select(x => x.Task));
        }

        Guid[] _jobIds;
        TaskCompletionSource<ConsumeContext<JobCompleted>>[] _completed;
        TaskCompletionSource<ConsumeContext<JobSubmitted>>[] _submitted;
        TaskCompletionSource<ConsumeContext<JobStarted>>[] _started;

        const int Count = 10;

        [OneTimeSetUp]
        public async Task Arrange()
        {
            _jobIds = new Guid[Count];
            for (var i = 0; i < _jobIds.Length; i++)
                _jobIds[i] = NewId.NextGuid();
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
                    x.SlotWaitTime = TimeSpan.FromSeconds(1);
                    x.SagaPartitionCount = 16;
                    x.FinalizeCompleted = true;

                    x.UseAzureTableSagaRepository(() => TestCloudTable);
                });

                instance.ReceiveEndpoint(instance.EndpointNameFormatter.Message<CrunchTheNumbers>(), e =>
                {
                    e.Consumer(() => new CrunchTheNumbersConsumer(), cfg =>
                    {
                        cfg.Options<JobOptions<CrunchTheNumbers>>(o => o.SetJobTimeout(TimeSpan.FromSeconds(90)).SetConcurrentJobLimit(3));
                    });
                    e.UseMessageScheduler(e.InputAddress);
                });
            });
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _submitted = new TaskCompletionSource<ConsumeContext<JobSubmitted>>[Count];
            _started = new TaskCompletionSource<ConsumeContext<JobStarted>>[Count];
            _completed = new TaskCompletionSource<ConsumeContext<JobCompleted>>[Count];

            for (var i = 0; i < Count; i++)
            {
                _submitted[i] = GetTask<ConsumeContext<JobSubmitted>>();
                _started[i] = GetTask<ConsumeContext<JobStarted>>();
                _completed[i] = GetTask<ConsumeContext<JobCompleted>>();
            }

            configurator.Handler<JobSubmitted>(context =>
            {
                for (var i = 0; i < Count; i++)
                {
                    if (_jobIds[i] == context.Message.JobId)
                        _submitted[i].TrySetResult(context);
                }

                return Task.CompletedTask;
            });

            configurator.Handler<JobStarted>(context =>
            {
                for (var i = 0; i < Count; i++)
                {
                    if (_jobIds[i] == context.Message.JobId)
                        _started[i].TrySetResult(context);
                }

                return Task.CompletedTask;
            });

            configurator.Handler<JobCompleted>(context =>
            {
                for (var i = 0; i < Count; i++)
                {
                    if (_jobIds[i] == context.Message.JobId)
                        _completed[i].TrySetResult(context);
                }

                return Task.CompletedTask;
            });
        }
    }
}
