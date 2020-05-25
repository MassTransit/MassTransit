namespace MassTransit.Turnout.Components
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;
    using Contracts;
    using MassTransit.Contracts.Turnout;
    using Metadata;


    public class JobService :
        IJobService
    {
        readonly IJobRegistry _registry;
        bool _stopping;

        public JobService(IJobRegistry jobRegistry, Uri instanceAddress)
        {
            _registry = jobRegistry;

            InstanceAddress = instanceAddress;
        }

        public Uri InstanceAddress { get; }

        public async Task<JobHandle> StartJob<T>(ConsumeContext<StartJob> context, T job, IJobFactory<T> jobFactory, TimeSpan timeout)
            where T : class
        {
            if (_stopping)
                throw new InvalidOperationException("The job service is stopping.");

            var startJob = context.Message;

            var jobContext = new ConsumeJobContext<T>(context, InstanceAddress, startJob.JobId, startJob.AttemptId, startJob.RetryAttempt, job, timeout);

            LogContext.Debug?.Log("Executing job: {JobType} {JobId} ({RetryAttempt})", TypeMetadataCache<T>.ShortName, startJob.JobId,
                startJob.RetryAttempt);

            var executeJobPipe = new ExecuteJobPipe<T>(jobFactory);

            var jobTask = executeJobPipe.Send(jobContext);

            var jobHandle = new ConsumerJobHandle<T>(jobContext, jobTask);

            _registry.Add(jobHandle);

            return jobHandle;
        }

        public async Task Stop()
        {
            _stopping = true;

            ICollection<JobHandle> pendingJobs = _registry.GetAll();

            foreach (var jobHandle in pendingJobs)
            {
                if (jobHandle.Status == JobStatus.Created || jobHandle.Status == JobStatus.Running)
                {
                    try
                    {
                        LogContext.Debug?.Log("Cancelling job: {JobId}", jobHandle.JobId);

                        await jobHandle.Cancel().ConfigureAwait(false);

                        _registry.TryRemoveJob(jobHandle.JobId, out _);
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
