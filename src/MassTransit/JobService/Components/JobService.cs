namespace MassTransit.JobService.Components
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using MassTransit.Contracts.JobService;
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

        public void Add(JobHandle jobReference)
        {
            if (!_jobs.TryAdd(jobReference.JobId, jobReference))
                throw new JobAlreadyExistsException(jobReference.JobId);
        }

        public bool TryRemoveJob(Guid jobId, out JobHandle jobHandle)
        {
            var removed = _jobs.TryRemove(jobId, out jobHandle);
            if (removed)
            {
                LogContext.Debug?.Log("Removed job: {JobId} ({JobStatus})", jobId, jobHandle.Status);

                return true;
            }

            return false;
        }

        public Uri InstanceAddress { get; }

        public async Task<JobHandle> StartJob<T>(ConsumeContext<StartJob> context, T job, IPipe<ConsumeContext<T>> jobPipe, TimeSpan timeout)
            where T : class
        {
            var startJob = context.Message;

            var jobContext = new ConsumeJobContext<T>(context, InstanceAddress, startJob.JobId, startJob.AttemptId, startJob.RetryAttempt, job, timeout);

            LogContext.Debug?.Log("Executing job: {JobType} {JobId} ({RetryAttempt})", TypeMetadataCache<T>.ShortName, startJob.JobId,
                startJob.RetryAttempt);

            ConsumeContext<T> jobConsumeContext = new MessageConsumeContext<T>(context, job);
            jobConsumeContext.AddOrUpdatePayload<JobContext<T>>(() => jobContext, existing => jobContext);

            var jobTask = jobPipe.Send(jobConsumeContext);

            var jobHandle = new ConsumerJobHandle<T>(jobContext, jobTask);

            Add(jobHandle);

            return jobHandle;
        }

        public async Task Stop()
        {
            ICollection<JobHandle> pendingJobs = _jobs.Values;

            foreach (var jobHandle in pendingJobs)
            {
                if (jobHandle.Status == JobStatus.Created || jobHandle.Status == JobStatus.Running)
                {
                    try
                    {
                        LogContext.Debug?.Log("Cancelling job: {JobId}", jobHandle.JobId);

                        await jobHandle.Cancel().ConfigureAwait(false);

                        TryRemoveJob(jobHandle.JobId, out _);
                    }
                    catch (Exception ex)
                    {
                        LogContext.Error?.Log(ex, "Cancel job faulted: {JobId}", jobHandle.JobId);
                    }
                }
            }
        }
    }
}
