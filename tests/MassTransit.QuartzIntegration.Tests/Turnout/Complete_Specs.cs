namespace MassTransit.QuartzIntegration.Tests.Turnout
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Conductor.Configuration;
    using Contracts.Turnout;
    using Definition;
    using NUnit.Framework;
    using Util;


    public interface CrunchTheNumbers
    {
        TimeSpan Duration { get; }
    }


    [TestFixture]
    public class Submitting_a_job_to_turnout :
        QuartzInMemoryTestFixture
    {
        [Test]
        [Order(1)]
        public async Task Should_get_the_job_accepted()
        {
            var serviceClient = Bus.CreateServiceClient();

            IRequestClient<SubmitJob<CrunchTheNumbers>> requestClient = serviceClient.CreateRequestClient<SubmitJob<CrunchTheNumbers>>();

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

        [OneTimeSetUp]
        public async Task Arrange()
        {
            _jobId = NewId.NextGuid();
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

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _submitted = Handled<JobSubmitted>(configurator, context => context.Message.JobId == _jobId);
            _started = Handled<JobStarted>(configurator, context => context.Message.JobId == _jobId);
            _completed = Handled<JobCompleted>(configurator, context => context.Message.JobId == _jobId);
        }
    }


    [TestFixture]
    public class Submitting_a_job_to_turnout_using_request_client :
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

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _submitted = Handled<JobSubmitted>(configurator, context => context.Message.JobId == _jobId);
            _started = Handled<JobStarted>(configurator, context => context.Message.JobId == _jobId);
            _completed = Handled<JobCompleted>(configurator, context => context.Message.JobId == _jobId);
        }
    }


    [TestFixture]
    [Category("SlowAF")]
    public class Submitting_a_bunch_of_jobs :
        QuartzInMemoryTestFixture
    {
        [Test]
        [Order(1)]
        public async Task Should_get_the_job_accepted()
        {
            var serviceClient = Bus.CreateServiceClient();

            IRequestClient<SubmitJob<CrunchTheNumbers>> requestClient = serviceClient.CreateRequestClient<SubmitJob<CrunchTheNumbers>>();

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
                .EnableInstanceEndpoint()
                .SetEndpointNameFormatter(KebabCaseEndpointNameFormatter.Instance);

            configurator.ServiceInstance(options, instance =>
            {
                instance.Turnout(x =>
                {
                    x.JobSlotWaitTime = TimeSpan.FromSeconds(1);
                    x.Job<CrunchTheNumbers>(cfg =>
                    {
                        cfg.ConcurrentJobLimit = 3;

                        cfg.JobTimeout = TimeSpan.FromSeconds(90);

                        cfg.SetJobFactory(async context =>
                        {
                            await Task.Delay(context.Job.Duration);
                        });
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

                return TaskUtil.Completed;
            });

            configurator.Handler<JobStarted>(context =>
            {
                for (var i = 0; i < Count; i++)
                {
                    if (_jobIds[i] == context.Message.JobId)
                        _started[i].TrySetResult(context);
                }

                return TaskUtil.Completed;
            });

            configurator.Handler<JobCompleted>(context =>
            {
                for (var i = 0; i < Count; i++)
                {
                    if (_jobIds[i] == context.Message.JobId)
                        _completed[i].TrySetResult(context);
                }

                return TaskUtil.Completed;
            });
        }
    }
}
