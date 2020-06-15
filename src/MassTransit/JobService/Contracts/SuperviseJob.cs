namespace MassTransit.JobService.Contracts
{
    using System;
    using Components;


    /// <summary>
    /// Supervise an active job
    /// Sent to the node where the job is executing so that it can check the status
    /// of the job. Sent at an interval until the job is completed.
    /// </summary>
    public interface SuperviseJob<out T>
        where T : class
    {
        /// <summary>
        /// The job identifier
        /// </summary>
        Guid JobId { get; }

        /// <summary>
        /// The time of the last job status check
        /// </summary>
        DateTime LastUpdated { get; }

        /// <summary>
        /// The previous job status
        /// </summary>
        JobStatus LastStatus { get; }

        /// <summary>
        /// The job command, which initiated the job
        /// </summary>
        T Command { get; }
    }
}
