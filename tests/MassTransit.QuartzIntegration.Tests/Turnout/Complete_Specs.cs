namespace MassTransit.QuartzIntegration.Tests.Turnout
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts.JobService;
    using JobService;
    using NUnit.Framework;
    using Util;


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
        QuartzInMemoryTestFixture
    {
        [Test]
        [Order(1)]
        public async Task Should_get_the_job_accepted()
        {
            IRequestClient<SubmitJob<CrunchTheNumbers>> requestClient = Bus.CreateRequestClient<SubmitJob<CrunchTheNumbers>>();

            Response<JobSubmissionAccepted> response = await requestClient.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = _jobId,
                Job = new {Duration = TimeSpan.FromSeconds(1)}
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

        Guid _jobId;
        Task<ConsumeContext<JobCompleted>> _completed;
        Task<ConsumeContext<JobSubmitted>> _submitted;
        Task<ConsumeContext<JobStarted>> _started;

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
                instance.ConfigureJobServiceEndpoints();

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


    [TestFixture]
    [Category("Flaky")]
    public class Submitting_a_job_to_turnout_using_request_client :
        QuartzInMemoryTestFixture
    {
        [Test]
        [Order(1)]
        public async Task Should_get_the_job_accepted()
        {
            IRequestClient<SubmitJob<CrunchTheNumbers>> requestClient = Bus.CreateRequestClient<SubmitJob<CrunchTheNumbers>>(_serviceAddress);

            Response<JobSubmissionAccepted> response = await requestClient.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = _jobId,
                Job = new {Duration = TimeSpan.FromSeconds(1)}
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
        Uri _serviceAddress;

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
                instance.ConfigureJobServiceEndpoints();

                instance.ReceiveEndpoint(instance.EndpointNameFormatter.Message<CrunchTheNumbers>(), e =>
                {
                    e.Consumer(() => new CrunchTheNumbersConsumer(), cfg =>
                    {
                        cfg.Options<JobOptions<CrunchTheNumbers>>(jobOptions => jobOptions.SetJobTimeout(TimeSpan.FromSeconds(90)));
                    });

                    _serviceAddress = e.InputAddress;
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


    [TestFixture]
    [Category("Flaky")]
    public class Submitting_a_bunch_of_jobs :
        QuartzInMemoryTestFixture
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
                    Job = new {Duration = TimeSpan.FromSeconds(1)}
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
                });

                instance.ReceiveEndpoint(instance.EndpointNameFormatter.Message<CrunchTheNumbers>(), e =>
                {
                    e.Consumer(() => new CrunchTheNumbersConsumer(), cfg =>
                    {
                        cfg.Options<JobOptions<CrunchTheNumbers>>(o => o.SetJobTimeout(TimeSpan.FromSeconds(90)).SetConcurrentJobLimit(3));
                    });
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


    [TestFixture]
    [Category("Flaky")]
    public class Submitting_a_job_to_turnout_with_status_checks :
        QuartzInMemoryTestFixture
    {
        public Submitting_a_job_to_turnout_with_status_checks()
        {
            TestTimeout = TimeSpan.FromMinutes(5);
            TestInactivityTimeout = TimeSpan.FromMinutes(1);
        }

        [Test]
        [Order(1)]
        public async Task Should_get_the_job_accepted()
        {
            IRequestClient<SubmitJob<CrunchTheNumbers>> requestClient = Bus.CreateRequestClient<SubmitJob<CrunchTheNumbers>>();

            Response<JobSubmissionAccepted> response = await requestClient.GetResponse<JobSubmissionAccepted>(new
            {
                JobId = _jobId,
                Job = new {Duration = TimeSpan.FromMinutes(3.5)}
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

        Guid _jobId;
        Task<ConsumeContext<JobCompleted>> _completed;
        Task<ConsumeContext<JobSubmitted>> _submitted;
        Task<ConsumeContext<JobStarted>> _started;

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
                instance.ConfigureJobServiceEndpoints();

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
}
