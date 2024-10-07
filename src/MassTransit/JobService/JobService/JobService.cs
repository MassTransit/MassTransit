namespace MassTransit.JobService;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Configuration;
using Consumer;
using Contracts.JobService;
using Messages;
using Middleware;


public class JobService :
    IJobService
{
    readonly ConcurrentDictionary<Guid, JobHandle> _jobs;
    readonly Dictionary<Type, IJobTypeRegistration> _jobTypes;
    Timer _heartbeat;

    public JobService(JobServiceSettings settings)
    {
        Settings = settings;

        _jobTypes = new Dictionary<Type, IJobTypeRegistration>();
        _jobs = new ConcurrentDictionary<Guid, JobHandle>();
    }

    public JobServiceSettings Settings { get; }

    public Uri InstanceAddress => Settings.InstanceAddress;

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

    public async Task<JobHandle> StartJob<T>(ConsumeContext<StartJob> context, T job, IPipe<ConsumeContext<T>> jobPipe, JobOptions<T> jobOptions)
        where T : class
    {
        var startJob = context.Message;

        if (_jobs.ContainsKey(startJob.JobId))
            throw new JobAlreadyExistsException(startJob.JobId);

        var jobContext = new ConsumeJobContext<T>(context, InstanceAddress, job, jobOptions);

        LogContext.Debug?.Log("Executing job: {JobType} {JobId} ({RetryAttempt})", TypeCache<T>.ShortName, startJob.JobId,
            startJob.RetryAttempt);

        var jobTask = jobPipe.Send(jobContext);

        var jobHandle = new ConsumerJobHandle<T>(jobContext, jobTask, jobOptions.JobCancellationTimeout);

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
            if (!jobHandle.JobTask.IsCompleted)
            {
                try
                {
                    LogContext.Debug?.Log("Canceling job: {JobId}", jobHandle.JobId);

                    await jobHandle.Cancel(JobCancellationReasons.Shutdown).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    LogContext.Error?.Log(ex, "Cancel job faulted: {JobId}", jobHandle.JobId);
                }
            }

            if (TryRemoveJob(jobHandle.JobId, out _))
                await jobHandle.DisposeAsync().ConfigureAwait(false);
        }

        await Task.WhenAll(_jobTypes.Values.Select(x => x.PublishJobInstanceStopped(publishEndpoint))).ConfigureAwait(false);
    }

    public void RegisterJobType<T>(IReceiveEndpointConfigurator configurator, JobOptions<T> options, Guid jobTypeId, string jobTypeName)
        where T : class
    {
        if (_jobTypes.ContainsKey(typeof(T)))
            throw new ConfigurationException($"A job type can only be registered once per service instance: {TypeCache<T>.ShortName}");

        _jobTypes.Add(typeof(T), new JobTypeRegistration<T>(options, InstanceAddress, jobTypeId, jobTypeName));
    }

    public async Task BusStarted(IPublishEndpoint publishEndpoint)
    {
        await Task.WhenAll(_jobTypes.Values.Select(x => x.PublishConcurrentJobLimit(publishEndpoint))).ConfigureAwait(false);

        void PublishHeartbeats(object state)
        {
            Task.Run(async () =>
            {
                try
                {
                    await Task.WhenAll(_jobTypes.Values.Select(x => x.PublishHeartbeat(publishEndpoint))).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    LogContext.Debug?.Log(exception, "Failed to publish heartbeat");
                }
            });
        }

        _heartbeat = new Timer(PublishHeartbeats, null, Settings.HeartbeatInterval, Settings.HeartbeatInterval);
    }

    public Guid GetJobTypeId<T>()
        where T : class
    {
        if (_jobTypes.TryGetValue(typeof(T), out var registration))
            return registration.JobTypeId;

        throw new ConfigurationException($"The job type was not registered: {TypeCache<T>.ShortName}");
    }

    public void ConfigureSuperviseJobConsumer(IReceiveEndpointConfigurator configurator)
    {
        var partition = new Middleware.Partitioner(16, new Murmur3UnsafeHashGenerator());

        configurator.UsePartitioner<CancelJobAttempt>(partition, p => p.Message.JobId);
        configurator.UsePartitioner<GetJobAttemptStatus>(partition, p => p.Message.JobId);

        var consumerFactory = new DelegateConsumerFactory<SuperviseJobConsumer>(() => new SuperviseJobConsumer(this));

        var consumerConfigurator = new ConsumerConfigurator<SuperviseJobConsumer>(consumerFactory, configurator);

        configurator.AddEndpointSpecification(consumerConfigurator);
    }

    void Add(JobHandle jobHandle)
    {
        if (!_jobs.TryAdd(jobHandle.JobId, jobHandle))
            throw new JobAlreadyExistsException(jobHandle.JobId);

        jobHandle.JobTask.ContinueWith(async innerTask =>
        {
            if (TryRemoveJob(jobHandle.JobId, out _))
                await jobHandle.DisposeAsync().ConfigureAwait(false);
        });
    }


    interface IJobTypeRegistration
    {
        Guid JobTypeId { get; }
        Task PublishConcurrentJobLimit(IPublishEndpoint publishEndpoint);
        Task PublishHeartbeat(IPublishEndpoint publishEndpoint);
        Task PublishJobInstanceStopped(IPublishEndpoint publishEndpoint);
    }


    class JobTypeRegistration<T> :
        IJobTypeRegistration
        where T : class
    {
        readonly Uri _instanceAddress;
        readonly JobOptions<T> _options;

        public JobTypeRegistration(JobOptions<T> options, Uri instanceAddress, Guid jobTypeId, string jobTypeName)
        {
            _options = options;
            _instanceAddress = instanceAddress;
            JobTypeId = jobTypeId;
            JobTypeName = string.IsNullOrWhiteSpace(options.JobTypeName) ? jobTypeName : options.JobTypeName;
        }

        string JobTypeName { get; }

        public Task PublishConcurrentJobLimit(IPublishEndpoint publishEndpoint)
        {
            LogContext.Debug?.Log("Job Service type: {JobType}", TypeCache<T>.ShortName);

            return PublishSetConcurrentJobLimit(publishEndpoint, ConcurrentLimitKind.Configured);
        }

        public Task PublishHeartbeat(IPublishEndpoint publishEndpoint)
        {
            return PublishSetConcurrentJobLimit(publishEndpoint, ConcurrentLimitKind.Heartbeat);
        }

        public Task PublishJobInstanceStopped(IPublishEndpoint publishEndpoint)
        {
            return PublishSetConcurrentJobLimit(publishEndpoint, ConcurrentLimitKind.Stopped);
        }

        public Guid JobTypeId { get; }

        Task PublishSetConcurrentJobLimit(IPublishEndpoint publishEndpoint, ConcurrentLimitKind kind)
        {
            return publishEndpoint.Publish<SetConcurrentJobLimit>(new SetConcurrentJobLimitCommand
            {
                JobTypeId = JobTypeId,
                JobTypeName = JobTypeName,
                InstanceAddress = _instanceAddress,
                ConcurrentJobLimit = _options.ConcurrentJobLimit,
                Kind = kind,
                JobTypeProperties = _options.JobTypeProperties.Properties,
                InstanceProperties = _options.InstanceProperties.Properties,
                GlobalConcurrentJobLimit = _options.GlobalConcurrentJobLimit
            });
        }
    }
}
