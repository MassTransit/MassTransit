namespace MassTransit.JobService.Components
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Contracts.JobService;
    using GreenPipes;
    using Metadata;


    public class JobService :
        IJobService
    {
        readonly ConcurrentDictionary<Guid, JobHandle> _jobs;
        readonly Dictionary<Type, IJobTypeRegistration> _jobTypes;
        readonly JobServiceOptions _options;
        Timer _heartbeat;

        public JobService(Uri instanceAddress, JobServiceOptions options)
        {
            _options = options;
            InstanceAddress = instanceAddress;

            _jobTypes = new Dictionary<Type, IJobTypeRegistration>();
            _jobs = new ConcurrentDictionary<Guid, JobHandle>();
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

            LogContext.Debug?.Log("Executing job: {JobType} {JobId} ({RetryAttempt})", TypeMetadataCache<T>.ShortName, startJob.JobId,
                startJob.RetryAttempt);

            var jobTask = jobPipe.Send(jobContext);

            var jobHandle = new ConsumerJobHandle<T>(jobContext, jobTask);

            Add(jobHandle);

            return jobHandle;
        }

        public async Task Stop(IBus bus)
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

            await Task.WhenAll(_jobTypes.Values.Select(x => x.PublishJobInstanceStopped(bus, InstanceAddress))).ConfigureAwait(false);
        }

        public void RegisterJobType<T>(IReceiveEndpointConfigurator configurator, JobOptions<T> options)
            where T : class
        {
            _jobTypes.Add(typeof(T), new JobTypeRegistration<T>(configurator, options));
        }

        public async Task BusStarted(IBus bus)
        {
            await Task.WhenAll(_jobTypes.Values.Select(x => x.PublishConcurrentJobLimit(bus, InstanceAddress))).ConfigureAwait(false);

            void PublishHeartbeats(object state)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await Task.WhenAll(_jobTypes.Values.Select(x => x.PublishHeartbeat(bus, InstanceAddress))).ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        LogContext.Debug?.Log(exception, "Failed to publish heartbeat");
                    }
                });
            }

            _heartbeat = new Timer(PublishHeartbeats, null, _options.HeartbeatInterval, _options.HeartbeatInterval);
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


        interface IJobTypeRegistration
        {
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

            public JobTypeRegistration(IReceiveEndpointConfigurator configurator, JobOptions<T> options)
            {
                _configurator = configurator;
                _options = options;
            }

            public Task PublishConcurrentJobLimit(IPublishEndpoint publishEndpoint, Uri instanceAddress)
            {
                LogContext.Debug?.Log("Job Service type: {JobType}", TypeMetadataCache<T>.ShortName);

                return publishEndpoint.Publish<SetConcurrentJobLimit>(new
                {
                    JobMetadataCache<T>.JobTypeId,
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
                    JobMetadataCache<T>.JobTypeId,
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
                    JobMetadataCache<T>.JobTypeId,
                    instanceAddress,
                    ServiceAddress = _configurator.InputAddress,
                    _options.ConcurrentJobLimit,
                    Kind = ConcurrentLimitKind.Stopped
                });
            }
        }
    }
}
