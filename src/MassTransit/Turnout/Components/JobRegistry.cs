namespace MassTransit.Turnout.Components
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Context;


    /// <summary>
    /// Maintains the jobs for a turnout
    /// </summary>
    public class JobRegistry :
        IJobRegistry
    {
        readonly ConcurrentDictionary<Guid, JobHandle> _jobs;

        public JobRegistry()
        {
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

        public ICollection<JobHandle> GetAll()
        {
            return _jobs.Values;
        }
    }
}
