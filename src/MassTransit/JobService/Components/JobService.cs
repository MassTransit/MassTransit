namespace MassTransit.JobService.Components
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;
    using Contracts.JobService;
    using GreenPipes;
    using Metadata;


    public class JobService :
        IJobService
    {
        readonly ConcurrentDictionary<Guid, JobHandle> _jobs;

        public JobService(Uri instanceAddress)
        {
            InstanceAddress = instanceAddress;

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

        public async Task Stop()
        {
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
    }
}
