namespace MassTransit.Contracts.JobService
{
    using System;


    /// <summary>
    /// Signals that the time to supervise a job has expired, and the instance should be checked
    /// </summary>
    public interface JobStatusCheckRequested
    {
        /// <summary>
        /// Identifies this attempt to run the job
        /// </summary>
        Guid AttemptId { get; }
    }
}
