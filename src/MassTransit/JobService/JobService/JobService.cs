namespace MassTransit.JobService
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Consumer;
    using Contracts.JobService;


    public class JobService :
        IJobService
    {
        readonly ConcurrentDictionary<Guid, JobHandle> _jobs;
        readonly Dictionary<Type, IJobTypeRegistration> _jobTypes;
        readonly JobServiceOptions _options;
        Timer _heartbeat;

        public JobService(IServiceInstanceConfigurator configurator, JobServiceOptions options)
        {
            _options = options;
            InstanceAddress = configurator.InstanceAddress;

            _jobTypes = new Dictionary<Type, IJobTypeRegistration>();
            _jobs = new ConcurrentDictionary<Guid, JobHandle>();

            ConfigureSuperviseJobConsumer(configurator.InstanceEndpointConfigurator);
        }

        public bool TryGetJob(Guid jobId, out JobHandle jobReference)
        {
            return _jobs.TryGetValue(jobId, out jobReference);
        }

        public bool TryRemoveJob(Guid jobId, out JobHandle jobHandle)
        {
            var removed = _jobs.TryRemove(jobId, out jobHandle);
            if (removed)
            {
                LogContext.Debug?.Log("Removed job: {JobId} ({Status})", jobId, jobHandle.JobTask.Status);

                return true;
            }

            return false;
        }

        public Uri InstanceAddress { get; }

        public async Task<JobHandle> StartJob<T>(ConsumeContext<StartJob> context, T job, IPipe<ConsumeContext<T>> jobPipe, TimeSpan timeout)
            where T : class
        {
            var startJob = context.Message;

            if (_jobs.ContainsKey(startJob.JobId))
                throw new JobAlreadyExistsException(startJob.JobId);

            var jobContext = new ConsumeJobContext<T>(context, InstanceAddress, startJob.JobId, startJob.AttemptId, startJob.RetryAttempt, job, timeout);

            LogContext.Debug?.Log("Executing job: {JobType} {JobId} ({RetryAttempt})", TypeCache<T>.ShortName, startJob.JobId,
                startJob.RetryAttempt);

            var jobTask = jobPipe.Send(jobContext);

            var jobHandle = new ConsumerJobHandle<T>(jobContext, jobTask);

            Add(jobHandle);

            return jobHandle;
        }

        public async Task Stop(IPublishEndpoint publishEndpoint)
        {
            if (_heartbeat != null)
            {
                _heartbeat.Dispose();
                _heartbeat = null;
            }

            ICollection<JobHandle> pendingJobs = _jobs.Values;

            foreach (var jobHandle in pendingJobs)
            {
                if (jobHandle.JobTask.IsCompleted)
                    continue;

                try
                {
                    LogContext.Debug?.Log("Canceling job: {JobId}", jobHandle.JobId);

                    await jobHandle.Cancel().ConfigureAwait(false);

                    TryRemoveJob(jobHandle.JobId, out _);
                }
                catch (Exception ex)
                {
                    LogContext.Error?.Log(ex, "Cancel job faulted: {JobId}", jobHandle.JobId);
                }
            }

            await Task.WhenAll(_jobTypes.Values.Select(x => x.PublishJobInstanceStopped(publishEndpoint, InstanceAddress))).ConfigureAwait(false);
        }

        public void RegisterJobType<T>(IReceiveEndpointConfigurator configurator, JobOptions<T> options, Guid jobTypeId)
            where T : class
        {
            if (_jobTypes.ContainsKey(typeof(T)))
                throw new ConfigurationException($"A job type can only be registered once per service instance: {TypeCache<T>.ShortName}");

            _jobTypes.Add(typeof(T), new JobTypeRegistration<T>(configurator, options, jobTypeId));
        }

        public async Task BusStarted(IPublishEndpoint publishEndpoint)
        {
            await Task.WhenAll(_jobTypes.Values.Select(x => x.PublishConcurrentJobLimit(publishEndpoint, InstanceAddress))).ConfigureAwait(false);

            void PublishHeartbeats(object state)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await Task.WhenAll(_jobTypes.Values.Select(x => x.PublishHeartbeat(publishEndpoint, InstanceAddress))).ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        LogContext.Debug?.Log(exception, "Failed to publish heartbeat");
                    }
                });
            }

            _heartbeat = new Timer(PublishHeartbeats, null, _options.HeartbeatInterval, _options.HeartbeatInterval);
        }

        public Guid GetJobTypeId<T>()
            where T : class
        {
            if (_jobTypes.TryGetValue(typeof(T), out var registration))
                return registration.JobTypeId;

            throw new ConfigurationException($"The job type was not registered: {TypeCache<T>.ShortName}");
        }

        void Add(JobHandle jobHandle)
        {
            if (!_jobs.TryAdd(jobHandle.JobId, jobHandle))
                throw new JobAlreadyExistsException(jobHandle.JobId);

            jobHandle.JobTask.ContinueWith(innerTask =>
            {
                TryRemoveJob(jobHandle.JobId, out _);
            });
        }

        void ConfigureSuperviseJobConsumer(IReceiveEndpointConfigurator configurator)
        {
            var consumerFactory = new DelegateConsumerFactory<SuperviseJobConsumer>(() => new SuperviseJobConsumer(this));

            var consumerConfigurator = new ConsumerConfigurator<SuperviseJobConsumer>(consumerFactory, configurator);

            configurator.AddEndpointSpecification(consumerConfigurator);
        }


        interface IJobTypeRegistration
        {
            Guid JobTypeId { get; }
            Task PublishConcurrentJobLimit(IPublishEndpoint publishEndpoint, Uri instanceAddress);
            Task PublishHeartbeat(IPublishEndpoint publishEndpoint, Uri instanceAddress);
            Task PublishJobInstanceStopped(IPublishEndpoint publishEndpoint, Uri instanceAddress);
        }


        class JobTypeRegistration<T> :
            IJobTypeRegistration
            where T : class
        {
            readonly IReceiveEndpointConfigurator _configurator;
            readonly JobOptions<T> _options;

            public JobTypeRegistration(IReceiveEndpointConfigurator configurator, JobOptions<T> options, Guid jobTypeId)
            {
                _configurator = configurator;
                _options = options;
                JobTypeId = jobTypeId;
            }

            public Task PublishConcurrentJobLimit(IPublishEndpoint publishEndpoint, Uri instanceAddress)
            {
                LogContext.Debug?.Log("Job Service type: {JobType}", TypeCache<T>.ShortName);

                return publishEndpoint.Publish<SetConcurrentJobLimit>(new
                {
                    JobTypeId,
                    instanceAddress,
                    ServiceAddress = _configurator.InputAddress,
                    _options.ConcurrentJobLimit,
                    Kind = ConcurrentLimitKind.Configured
                });
            }

            public Task PublishHeartbeat(IPublishEndpoint publishEndpoint, Uri instanceAddress)
            {
                return publishEndpoint.Publish<SetConcurrentJobLimit>(new
                {
                    JobTypeId,
                    instanceAddress,
                    ServiceAddress = _configurator.InputAddress,
                    _options.ConcurrentJobLimit,
                    Kind = ConcurrentLimitKind.Heartbeat
                });
            }

            public Task PublishJobInstanceStopped(IPublishEndpoint publishEndpoint, Uri instanceAddress)
            {
                return publishEndpoint.Publish<SetConcurrentJobLimit>(new
                {
                    JobTypeId,
                    instanceAddress,
                    ServiceAddress = _configurator.InputAddress,
                    _options.ConcurrentJobLimit,
                    Kind = ConcurrentLimitKind.Stopped
                });
            }

            public Guid JobTypeId { get; }
        }
    }
}
