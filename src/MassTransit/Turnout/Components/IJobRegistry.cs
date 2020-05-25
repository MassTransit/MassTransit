namespace MassTransit.Turnout.Components
{
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Contains the active jobs for the receive endpoint which have not been acknowledged
    /// </summary>
    public interface IJobRegistry
    {
        bool TryGetJob(Guid jobId, out JobHandle jobReference);

        /// <summary>
        /// Add a job to the registry
        /// </summary>
        /// <param name="jobReference"></param>
        void Add(JobHandle jobReference);

        /// <summary>
        /// Remove the job from the roster
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="jobHandle"></param>
        bool TryRemoveJob(Guid jobId, out JobHandle jobHandle);

        /// <summary>
        /// Return all pending jobs from the registry
        /// </summary>
        /// <returns></returns>
        ICollection<JobHandle> GetAll();
    }
}
