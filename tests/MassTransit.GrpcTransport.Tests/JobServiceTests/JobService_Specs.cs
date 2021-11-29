namespace MassTransit.GrpcTransport.Tests.JobServiceTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using MassTransit.Contracts.JobService;
    using NUnit.Framework;


    [TestFixture]
    public class A_single_job_service_instance :
        GrpcClientTestFixture
    {
        [Test]
        [Order(1)]
        public async Task Should_get_the_job_accepted()
        {
            IRequestClient<SubmitJob<EncodeVideo>> requestClient = ClientBus.CreateRequestClient<SubmitJob<EncodeVideo>>();

            Response<JobSubmissionAccepted> response =
                await requestClient.GetResponse<JobSubmissionAccepted>(new
                {
                    JobId = _jobId,
                    Job = new
                    {
                        VideoId = _jobId,
                        Path = "C:\\Downloads\\RickRoll.mp4",
                        Duration = 1
                    }
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
            ConsumeContext<JobCompleted<EncodeVideo>> completed = await _completedT;
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

        public A_single_job_service_instance()
        {
            TestTimeout = TimeSpan.FromSeconds(5);
            _jobServiceOptions = new JobServiceOptions();
        }

        readonly Guid _jobId = NewId.NextGuid();
        readonly JobServiceOptions _jobServiceOptions;

        Task<ConsumeContext<JobCompleted>> _completed;
        Task<ConsumeContext<JobSubmitted>> _submitted;
        Task<ConsumeContext<JobStarted>> _started;
        Task<ConsumeContext<JobCompleted<EncodeVideo>>> _completedT;

        protected override void ConfigureGrpcClientReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
            _submitted = Handled<JobSubmitted>(configurator, context => context.Message.JobId == _jobId);
            _started = Handled<JobStarted>(configurator, context => context.Message.JobId == _jobId);
            _completed = Handled<JobCompleted>(configurator, context => context.Message.JobId == _jobId);
            _completedT = Handled<JobCompleted<EncodeVideo>>(configurator, context => context.Message.JobId == _jobId);
        }

        protected override void ConfigureGrpcClientBus(IGrpcBusFactoryConfigurator configurator)
        {
            base.ConfigureGrpcClientBus(configurator);

            configurator.UseDelayedMessageScheduler();

            var options = new ServiceInstanceOptions()
                .SetEndpointNameFormatter(KebabCaseEndpointNameFormatter.Instance);

            configurator.ServiceInstance(options, instance =>
            {
                instance.ConfigureJobService(_jobServiceOptions);

                instance.ReceiveEndpoint(instance.EndpointNameFormatter.Message<EncodeVideo>(), e =>
                {
                    e.Consumer(() => new EncodeVideoConsumer(LoggerFactory.CreateLogger("EncodeVideo")), x =>
                    {
                        x.Options<JobOptions<EncodeVideo>>(x => x.SetConcurrentJobLimit(2));
                    });
                });
            });
        }

        protected override void ConfigureGrpcBus(IGrpcBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();

            var options = new ServiceInstanceOptions()
                .SetEndpointNameFormatter(KebabCaseEndpointNameFormatter.Instance);

            configurator.ServiceInstance(options, instance =>
            {
                instance.ConfigureJobServiceEndpoints(_jobServiceOptions);
            });
        }

        protected override void ConfigureGrpcReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
        }
    }


    [TestFixture]
    public class A_single_job_service_instance_running_multiple_jobs :
        GrpcClientTestFixture
    {
        [Test]
        [Order(1)]
        public async Task Should_get_the_job_accepted()
        {
            IRequestClient<SubmitJob<EncodeVideo>> requestClient = ClientBus.CreateRequestClient<SubmitJob<EncodeVideo>>();

            for (var i = 0; i < Count; i++)
            {
                Response<JobSubmissionAccepted> response =
                    await requestClient.GetResponse<JobSubmissionAccepted>(new
                    {
                        JobId = _jobIds[i],
                        Job = new
                        {
                            VideoId = _jobIds[i],
                            Path = "C:\\Downloads\\RickRoll.mp4",
                            Duration = 1
                        }
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

        public A_single_job_service_instance_running_multiple_jobs()
        {
            TestTimeout = TimeSpan.FromSeconds(10);
            _jobServiceOptions = new JobServiceOptions();
        }

        Guid[] _jobIds;
        TaskCompletionSource<ConsumeContext<JobCompleted>>[] _completed;
        TaskCompletionSource<ConsumeContext<JobSubmitted>>[] _submitted;
        TaskCompletionSource<ConsumeContext<JobStarted>>[] _started;

        readonly JobServiceOptions _jobServiceOptions;
        const int Count = 10;

        [OneTimeSetUp]
        public async Task Arrange()
        {
            _jobIds = new Guid[Count];
            for (var i = 0; i < _jobIds.Length; i++)
                _jobIds[i] = NewId.NextGuid();
        }

        protected override void ConfigureGrpcClientReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
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

        protected override void ConfigureGrpcClientBus(IGrpcBusFactoryConfigurator configurator)
        {
            base.ConfigureGrpcClientBus(configurator);

            configurator.UseDelayedMessageScheduler();

            var options = new ServiceInstanceOptions()
                .SetEndpointNameFormatter(KebabCaseEndpointNameFormatter.Instance);

            configurator.ServiceInstance(options, instance =>
            {
                instance.ConfigureJobService(_jobServiceOptions);

                instance.ReceiveEndpoint(instance.EndpointNameFormatter.Message<EncodeVideo>(), e =>
                {
                    e.Consumer(() => new EncodeVideoConsumer(LoggerFactory.CreateLogger("EncodeVideo")), x =>
                    {
                        x.Options<JobOptions<EncodeVideo>>(options => options.SetConcurrentJobLimit(5));
                    });
                });
            });
        }

        protected override void ConfigureGrpcBus(IGrpcBusFactoryConfigurator configurator)
        {
            configurator.UseDelayedMessageScheduler();

            var options = new ServiceInstanceOptions()
                .SetEndpointNameFormatter(KebabCaseEndpointNameFormatter.Instance);

            configurator.ServiceInstance(options, instance =>
            {
                instance.ConfigureJobServiceEndpoints(_jobServiceOptions);

                instance.ReceiveEndpoint(instance.EndpointNameFormatter.Message<EncodeVideo>(), e =>
                {
                    e.Consumer(() => new EncodeVideoConsumer(LoggerFactory.CreateLogger("EncodeVideo")), x =>
                    {
                        x.Options<JobOptions<EncodeVideo>>(o => o.SetConcurrentJobLimit(5));
                    });
                });
            });
        }

        protected override void ConfigureGrpcReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
        }
    }
}
